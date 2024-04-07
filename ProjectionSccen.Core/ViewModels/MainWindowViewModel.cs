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
using ProjectionSccen.Core.Logs;
using ProjectionSccen.Core.Manager;
using ProjectionSccen.Core.Models;
using ProjectionSccen.Core.Models.NetModel;
using ProjectionSccen.Core.Service.NetService;
using ProjectionSccen.Core.View;
using ProjectionSccen.Core.ViewPage;
 

namespace ProjectionSccen.Core.ViewModels{

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
                OnPropertyChanged("TitleFontLength");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<Students> _studentDatas = new ObservableCollection<Students>();

        /// <summary>
        /// 人员信息
        /// </summary>
        public ObservableCollection<Students> StudentDatas
        {
            get => _studentDatas;
            set
            {
                _studentDatas = value;
                OnPropertyChanged("Students");
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
            set
            {
                projectName = value;
                OnPropertyChanged("ProjectName");
            }
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
            this.SetView(new ThirdPeopleViewModel(), typeof(ThridtyPeople));
            this.SetView(new FivetyPeopleViewModel(), typeof(FivetyPeople));
            EscClickCommand = new BaseCommand(obj => EscCurrentData(obj));
            CloseCommand = new BaseCommand(o => EscCurrentData(o));
            RankingClickCommand = new BaseCommand(obj => RankingCommand(obj));
            if (_netService.IsConnection)
            {
                _netService.ListenMessage();
            }
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

        private JsonConfig JsonConfig;

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

        private NetService _netService;
        /// <summary>
        ///
        /// </summary>
        private void StartNetConnection()
        {
            if (!String.IsNullOrEmpty(IpAddress) && !String.IsNullOrEmpty(port.ToString()))
            {
                _netService = new NetService(IpAddress, port);
                _netService._logDataCallBack += _LogDataCallBack;
                _netService._HandleExamNumber += _HandleExamNumber;
                _netService._HandleCurrentStudentData += _netService__HandleCurrentStudentData;
                _netService.StartNetService();       
            }
        }

        private void _netService__HandleCurrentStudentData(List<StudentData> studentData, string examTime)
        {
            
            CurrentDateTime = examTime;
            if (studentData.Count > 0)
            {
                if(_studentDatas.Count > 0) _studentDatas.Clear();
                studentData = studentData.FindAll(a=>!string.IsNullOrEmpty(a.Name)&&!string.IsNullOrEmpty(a.School));
                foreach (var student in studentData)
                {
                    Students students = new Students()
                    {
                        Name = student.Name,
                        School = student.School,
                        Score = student.Score,

                    };   
                    _studentDatas.Add(students );
                }
            }
            if (machineNum == 2) TwoPeopleViewModel.InitCommand?.Execute(_studentDatas.ToList());
            else if (machineNum == 4) FourPeopleViewModel.InitCommand?.Execute(_studentDatas.ToList());
            else if (machineNum == 10) TenPeopleViewModel.InitCommand?.Execute(_studentDatas.ToList());
            else if (machineNum == 20) TwentyPeopleViewModel.InitCommand?.Execute(  _studentDatas.ToList());
            else if (machineNum == 30) ThirdPeopleViewModel.InitCommand?.Execute(_studentDatas.ToList());
            else if (machineNum == 50) FivetyPeopleViewModel.InitCommand?.Execute(_studentDatas.ToList());
        }

        /// <summary>
        /// 
        /// </summary>
        private int machineNumber;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="num"></param>
        private void _HandleExamNumber(int num)
        {
            SetMachineNum(num);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        private void _LogDataCallBack(string message)
        {
            LogService.Info(message);
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
            else if (num <= 30 && num > 20)
            {
                SetViewDataModelIndex(4);
                ThirdPeopleViewModel.SetProjects(JsonConfig.Project);
                ThirdPeopleViewModel.SetTitle(JsonConfig.TitleFontLength);
                machineNum = 30;
            }
            else
            {
                SetViewDataModelIndex(5);
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
            if (MessageBox.Show("确定退出软件？", "确认提醒", MessageBoxButton.YesNo, MessageBoxImage.Question) ==
                MessageBoxResult.Yes)
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
               _netService.CloseNetService();
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
                 
            }
            else
            {
                MainWindowBackGround = jsonConfig.MainWindowBackGround;
                
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
}