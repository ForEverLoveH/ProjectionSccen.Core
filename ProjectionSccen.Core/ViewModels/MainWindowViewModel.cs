using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using ProjectionSccen.Core.Manager;
using ProjectionSccen.Core.Models;
using ProjectionSccen.Core.Models.NetModel;
using ProjectionSccen.Core.View;
using ProjectionSccen.Core.ViewPage;
using static ProjectionSccen.Core.Manager.NetControllerManager;

namespace ProjectionSccen.Core.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    /// <summary>
    ///
    /// </summary>
    private string _mainWindowBackGround;

    /// <summary>
    ///
    /// </summary>
    public string MainWindowBackGround
    {
        get => _mainWindowBackGround;
        set
        {
            _mainWindowBackGround = value;
            OnPropertyChanged("MainWindowBackGround");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private string _gameTitle;

    /// <summary>
    ///
    /// </summary>
    public string GameTitle
    {
        get => _gameTitle;
        set
        {
            _gameTitle = value;
            OnPropertyChanged("GameTitle");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private double _titleFontLength;

    /// <summary>
    ///
    /// </summary>
    public double TitleFontLength
    {
        get => _titleFontLength;
        set
        {
            _titleFontLength = value;
            OnPropertyChanged("itleFontLength");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private ObservableCollection<StudentData> _studentDatas = new ObservableCollection<StudentData>();

    /// <summary>
    /// 人员信息
    /// </summary>
    public ObservableCollection<StudentData> StudentDatas
    {
        get => _studentDatas;
        set
        {
            _studentDatas = value;
            OnPropertyChanged("StudentDatas");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private string currentDateTime = "00:00";

    /// <summary>
    /// 时间
    /// </summary>
    public string CurrentDateTime
    {
        get => currentDateTime;
        set
        {
            currentDateTime = value;
            OnPropertyChanged("CurrentDateTime");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private ResourceDictionary Resources { get; set; } = new ResourceDictionary();

    /// <summary>
    ///
    /// </summary>
    private List<object> Models = new List<object>();

    /// <summary>
    ///
    /// </summary>
    private FrameworkElement window;

    /// <summary>
    ///
    /// </summary>
    public FrameworkElement Window
    {
        get => window;
        set
        {
            window = value;
            window.Resources = this.Resources;
        }
    }

    /// <summary>
    ///
    /// </summary>
    private string projectName;

    /// <summary>
    ///
    /// </summary>
    public string ProjectName
    {
        get { return projectName; }
        set { projectName = value; OnPropertyChanged("ProjectName"); }
    }

    /// <summary>
    ///
    /// </summary>
    private object viewModelContent;

    /// <summary>
    ///
    /// </summary>
    public object ViewModelContent
    {
        get => viewModelContent;
        set
        {
            viewModelContent = value;
            OnPropertyChanged("ViewModelContent");
        }
    }

    /// <summary>
    ///
    /// </summary>
    private string rankingBackGround;

    /// <summary>
    ///
    /// </summary>
    public string RankingBackGround
    {
        get => rankingBackGround;
        set
        {
            rankingBackGround = value;
            OnPropertyChanged("RankingBackGround");
        }
    }

    /// <summary>
    ///
    /// </summary>
    public MainWindowViewModel()
    {
        ReadJsonDataConfig();
        this.SetView(new TwoPeopleViewModel(), typeof(TwoPeople));
        this.SetView(new FourPeopleViewModel(), typeof(FourPeople));
        this.SetView(new TenPeopleViewModel(), typeof(TenPeople));
        this.SetView(new TwentyPeopleViewModel(), typeof(TwentyPeople));
        this.SetView(new FivetyPeopleViewModel(), typeof(FivetyPeople));
        EscClickCommand = new BaseCommand(obj => EscCurrentData(obj));
        CloseCommand = new BaseCommand(o => EscCurrentData(o));
        RankingClickCommand = new BaseCommand(obj => RankingCommand(obj));
    }

    /// <summary>
    ///
    /// </summary>
    public BaseCommand RankingClickCommand { get; set; }

    /// <summary>
    ///
    /// </summary>
    public BaseCommand CloseCommand { get; set; }

    /// <summary>
    ///
    /// </summary>
    public BaseCommand EscClickCommand { get; set; }

    /// <summary>
    ///
    /// </summary>
    private NetControllerManager netControllerManager;

    private bool connnection = false;

    /// <summary>
    /// ip地址
    /// </summary>
    private string IpAddress { get; set; }

    /// <summary>
    /// 端口
    /// </summary>
    private int port { get; set; }

    /// <summary>
    /// 测试是否结束
    /// </summary>
    private bool IsEnd { get; set; }

    private int machineNum { get; set; } = 0;

    /// <summary>
    ///
    /// </summary>
    /// <param name="index"></param>
    private void SetViewDataModelIndex(int index)
    {
        if (index >= 0 && index < this.Models.Count)
        {
            ViewModelContent = this.Models[index];
        }
    }

    private JsonConfig JsonConfig { get; set; }

    /// <summary>
    ///
    /// </summary>
    private void ReadJsonDataConfig()
    {
        string path = File.ReadAllText("Config/Config.json");
        JsonConfig = JsonDataManager.Instance.DeserializeObject<JsonConfig>(path);
        if (JsonConfig != null)
        {
            SetCurrentViewData(JsonConfig);
            StartNetConnection();
        }
        else
        {
            return;
        }
    }

    #region 网络端

    /// <summary>
    ///
    /// </summary>
    private void StartNetConnection()
    {
        if (!String.IsNullOrEmpty(IpAddress) && !String.IsNullOrEmpty(port.ToString()))
        {
            netControllerManager = new NetControllerManager(IpAddress, port);
            netControllerManager._HandleRecieveDataCallBack += _HandleRecieveDataCallBack;
            netControllerManager.DataCallBack += DataCallBack;
            netControllerManager.ConnectionServer();
            if (netControllerManager.IsConnection)
            {
                connnection = netControllerManager.IsConnection;
                StartDispather();
                
            }
        }
    }
    /// <summary>
    /// 第一个计时器(负责监听串口的连接)
    /// </summary>
    private DispatcherTimer dispatcherTimer1;
    private void StartDispather()
    {

        dispatcherTimer1 = new DispatcherTimer();
        dispatcherTimer1.Interval = new TimeSpan(0, 0, 5);
        dispatcherTimer1.Tick += new EventHandler(DispatcherTimer1_Tick);
        dispatcherTimer1.Start();
        
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DispatcherTimer1_Tick(object? sender, EventArgs e)
    {
        if (students.Count > 0)
        {
            for (int i = 0; i < students.Count; i++)
            {
                StudentDatas.Add(students[i]);  
            }
            if (machineNum == 2)
                TwoPeopleViewModel.InitCommand?.Execute(StudentDatas.ToList());
            else if (machineNum == 4)
            {
                FourPeopleViewModel.InitCommand?.Execute(StudentDatas.ToList());
            }
            else if (machineNum.Equals(10))
            {
                TenPeopleViewModel.InitCommand?.Execute(StudentDatas.ToList());
            }
            else if (machineNum.Equals(20))
            {
                TwentyPeopleViewModel.InitCommand?.Execute(StudentDatas.ToList());
            }
            else if (machineNum.Equals(50))
            {
                FivetyPeopleViewModel.InitCommand?.Execute(StudentDatas.ToList());
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    private void DataCallBack(string data)
    {

        string path = "Log/log.txt";
        if (!File.Exists(path))
        {
            File.Create(path);
        }
       
        using (StreamWriter streamWriter = new StreamWriter(path, true))
        {
            streamWriter.WriteAsync(data);
        }



    }

    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    private void _HandleRecieveDataCallBack(string data)
    {
        if (!string.IsNullOrEmpty(data))
        {
            var mess = JsonDataManager.Instance.DeserializeObject<ClientMessage>(data);
            HandleServerMessageData(mess);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    private void HandleServerMessageData(ClientMessage message)
    {
        if (message != null && message.messageData != null)
        {
            if (message.messageData.RspGetMachineNum != null)
            {
                HandleRspGetMachineNum(message.messageData.RspGetMachineNum);
            }
            if (message.messageData.RspGetStudentDataList != null)
            {
                HandleRspGetCurrentTestingStudents(message.messageData.RspGetStudentDataList);
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="message"></param>
    private void HandleRspGetCurrentTestingStudents(RspGetStudentDataList message)
    {
        List<string> name = new List<string>();
        IsEnd = message.IsExamEnd;
        List<StudentData> studentDatas = message.studentDataList;
        CurrentDateTime = message.examTime;
        if (!IsEnd)
        {
            UpDataCurrentStudents(studentDatas);
        }
        else
        {
            if(students != null)
            {
                students.Clear();
            }
            studentDatas.Sort((X, Y) => (X.Score.CompareTo(Y.Score)));
        }
    }

    private List<StudentData>students= new List<StudentData>();
    /// <summary>
    ///
    /// </summary>
    /// <param name="studentDatas"></param>
    private void UpDataCurrentStudents(List<StudentData> studentDatas)
    {
        if (machineNum == 0)
        {
            SetMachineNum(studentDatas.Count);
        }
        if (studentDatas.Count != machineNum)
        {
            for (int i = studentDatas.Count; i < machineNum; i++)
            {
                studentDatas.Add(new StudentData()
                {
                   
                    Name = "",
                    Score = "",
                    School="",
                }); ;
            }
        }
        if (studentDatas.Count > 0)
        {
            foreach (var student in studentDatas)
            {
                var stu = students.Find(a => a.Name.Equals(student.Name));
                if (stu == null)
                    students.Add(student);
                else
                {
                    stu.School = student.School;
                    stu.Score = student.Score;
                }
            }
             
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="messageMessageData"></param>
    private void HandleRspGetMachineNum(RspGetMachineNum message)
    {
        int num = message.num;
        SetMachineNum(num);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="num"></param>
    private void SetMachineNum(int num)
    {
        if (num <= 2)
        {
            SetViewDataModelIndex(0);
            TwoPeopleViewModel.SetLogo(JsonConfig.IsLogoActive);
            TwoPeopleViewModel.SetLen(JsonConfig.TitleFontLength);
            
            machineNum = 2;
        }
        else if (num > 2 && num <= 4)
        {
            SetViewDataModelIndex(1);
            FourPeopleViewModel.SetLogo(JsonConfig.IsLogoActive);
            FourPeopleViewModel.SetProjects(JsonConfig.Project);
            FourPeopleViewModel.SetTitleLen(JsonConfig.TitleFontLength);
            machineNum = 4;
        }
        else if (num > 4 && num <= 10)
        {
            SetViewDataModelIndex(2);
           
            TenPeopleViewModel.SetProjectName(JsonConfig.Project);
            TenPeopleViewModel.SetProjectLen(JsonConfig.TitleFontLength);
           machineNum = 10;
        }
        else if (num <= 20 && num > 10)
        {
            SetViewDataModelIndex(3);
            TwentyPeopleViewModel.SetProjects(JsonConfig.Project);
            TwentyPeopleViewModel.SetTitle(JsonConfig.TitleFontLength);
            machineNum = 20;

        }

        else
        {
            SetViewDataModelIndex(4);
            FivetyPeopleViewModel.SetProjects(JsonConfig.Project);
            FivetyPeopleViewModel.SetTitle(JsonConfig.TitleFontLength);
            machineNum = 50; 
        }
    }

    #endregion 网络端

    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    private void RankingCommand(object obj)
    {
        if (IsEnd)
        {
            if (StudentDatas.Count > 0)
            {
                StudentDatas.ToList().Sort((X, Y) => int.Parse(X.Score).CompareTo(int.Parse(Y.Score)));
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="value"></param>
    private void EscCurrentData(object value)
    {
        if (MessageBox.Show("确定退出软件？", "确认提醒", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            CloseNetConnection();
            Application.Current.Shutdown();
        }
    }

    /// <summary>
    /// 关闭网络
    /// </summary>
    private void CloseNetConnection()
    {
        if (connnection)
            netControllerManager.CloseNetConnection();
        connnection = false;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="jsonConfig"></param>
    private void SetCurrentViewData(JsonConfig jsonConfig)
    {
        if (jsonConfig.IsLogoActive)
        {
            MainWindowBackGround = jsonConfig.MainWindowLogoBackGround;
            RankingBackGround = jsonConfig.RankingLogoBackGround;
        }
        else
        {
            MainWindowBackGround = jsonConfig.MainWindowBackGround;
            RankingBackGround = jsonConfig.RankingBackGround;
        }
        GameTitle = jsonConfig.Title;
        TitleFontLength = jsonConfig.TitleFontLength;

        ProjectName = jsonConfig.Project;
        IpAddress = jsonConfig.IPAddress;
        port = jsonConfig.Port;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="viewModel"></param>
    /// <param name="type"></param>
    private void SetView(object viewModel, Type type)
    {
        DataTemplateKey key = new DataTemplateKey(viewModel.GetType());
        DataTemplate template = new DataTemplate(viewModel.GetType());
        template.VisualTree = new FrameworkElementFactory(type);
        this.Resources.Add(key, template);
        this.Models.Add(viewModel);
    }
}