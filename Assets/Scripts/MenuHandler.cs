using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Threading.Tasks;
using VRM;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;

public class MenuHandler : MonoBehaviour
{
    public GameObject MenuLoadVRM;
    public GameObject MenuPose;
    public GameObject ToggleSwitch;

    private bool grid_update_request = false;

    // prefab label
    private readonly string button_prefab_list = "PressableButtonHoloLens2_32x96_Load";
    private readonly string button_prefab_up = "PressableButtonHoloLens2Circular_32x32_Up";
    private readonly string button_prefab_down = "PressableButtonHoloLens2Circular_32x32_Down";
    private readonly string button_prefab_empty = "ButtonSpace";

    // Start is called before the first frame update
    void Start()
    {
        MenuLoadVRM.SetActive(false);
        MenuPose.SetActive(false);
        Debug.developerConsoleVisible = false;
    }

    private void Update()
    {
        if (grid_update_request)
        {
            // grid update
            MenuLoadVRM.GetComponent<GridObjectCollection>().UpdateCollection();
            MenuPose.GetComponent<GridObjectCollection>().UpdateCollection();
            grid_update_request = false;
        }
    }

    public void ManipulatorSwitch()
    {
        bool sw = ToggleSwitch.GetComponent<Interactable>().IsToggled;
        GameObject.Find("Model").GetComponent<BoundsControl>().enabled = sw;
    }

    public void VrmFileList(int page)
    {
        string[] fs = listVrmFile();
        foreach (Transform child in MenuLoadVRM.transform)
        {
            Destroy(child.gameObject);
        }
        int startindex = page * 3;
        int lastindex = startindex + 3;
        if (lastindex > fs.Length) lastindex = fs.Length;
        // prev button
        if (startindex == 0)
        {
            getButtonInstance(button_prefab_empty, MenuLoadVRM);
        }
        else
        {
            var instance = getButtonInstance(button_prefab_up, MenuLoadVRM);
            instance.GetComponent<Interactable>().OnClick.AddListener(() => VrmFileList(page - 1));
        }
        // file button
        for (int i = startindex; i < lastindex; i++)
        {
            var instance = getButtonInstance(button_prefab_list, MenuLoadVRM);
            ButtonConfigHelper buttonLabel = instance.GetComponent(typeof(ButtonConfigHelper)) as ButtonConfigHelper;
            buttonLabel.MainLabelText = System.IO.Path.GetFileName(fs[i]);
            string path = fs[i];
            instance.GetComponent<Interactable>().OnClick.AddListener(() => LoadVrm(path));
        }
        // next button
        if (page*3+3 >= fs.Length)
        {
            getButtonInstance(button_prefab_empty, MenuLoadVRM);
        }
        else
        {
            var instance = getButtonInstance(button_prefab_down, MenuLoadVRM);
            instance.GetComponent<Interactable>().OnClick.AddListener(() => VrmFileList(page + 1));
        }
        // done
        grid_update_request = true;
        MenuLoadVRM.SetActive(true);
    }

    private string[] listVrmFile()
    {
        string folderPath;
#if WINDOWS_UWP
        folderPath = Windows.Storage.KnownFolders.Objects3D.Path + "\\vrm\\";
#else
        folderPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "\\vrm\\";
#endif
        string[] fs = System.IO.Directory.GetFiles(@folderPath, "*.vrm", System.IO.SearchOption.AllDirectories);
        return fs;
    }

    private GameObject getButtonInstance(string name, GameObject parent)
    {
        GameObject obj = (GameObject)Resources.Load(name);
        GameObject instance = (GameObject)Instantiate(obj,
                                                  new Vector3(0.0f, 0.0f, 0.0f),
                                                  Quaternion.identity);
        instance.transform.parent = parent.transform;
        instance.transform.localScale = new Vector3(1, 1, 1);
        return instance;
    }

    public void LoadVrm(string path)
    {
        var old = GameObject.Find("VRM");
        if (old != null)
        {
            Destroy(old);
        }
        //
        UnityEngine.Debug.Log("loading: " + path);
        Task model = ImportModel(path);
        MenuLoadVRM.SetActive(false);
        //UpdateBoundingBox();
    }

    private async Task ImportModel(string modelPath)
    {
        var context = new VRMImporterContext();
        var bytes = File.ReadAllBytes(modelPath);
        context.ParseGlb(bytes);
        var meta = context.ReadMeta();
        //UnityEngine.Debug.LogFormat("loaded {0}", meta.Title);
        //
        await context.LoadAsyncTask();
        context.EnableUpdateWhenOffscreen();
        var root = context.Root;
        //root.transform.SetParent(transform, false);
        var parentModel = GameObject.Find("Model").transform;
        root.transform.position = parentModel.position;
        root.transform.rotation = parentModel.rotation;
        root.transform.localScale = parentModel.localScale;
        root.transform.parent = parentModel;
        GameObject.Find("secondary").SetActive(false);
        context.ShowMeshes();
    }

    void UpdateBoundingBox()
    {
        var mesh = GameObject.Find("VRM").GetComponentsInChildren<SkinnedMeshRenderer>();
        Bounds area = new Bounds();
        foreach (var item in mesh)
        {
            area.Encapsulate(item.bounds);
        }
        GameObject.Find("Model").GetComponent<BoxCollider>().size = new Vector3(area.size.x, area.size.y, area.size.z);
        GameObject.Find("Model").GetComponent<BoxCollider>().center = new Vector3(area.center.x, area.center.y, area.center.z);
    }
}
