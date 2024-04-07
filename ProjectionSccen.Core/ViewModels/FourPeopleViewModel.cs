using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ProjectionSccen.Core.Models;

namespace ProjectionSccen.Core.ViewModels{

    public class FourPeopleViewModel : ViewModelBase
    {
        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<Students> students = new ObservableCollection<Students>();

        /// <summary>
        ///
        /// </summary>
        public ObservableCollection<Students> Students
        {
            get { return students; }
            set
            {
                students = value;
                OnPropertyChanged("Students");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private Visibility isLogo;

        public Visibility IsLogo
        {
            get { return isLogo; }
            set
            {
                isLogo = value;
                OnPropertyChanged("IsLogo");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private static bool isLogoActive;

        /// <summary>
        ///
        /// </summary>
        /// <param name="flag"></param>
        public static void SetLogo(bool flag)
        {
            isLogoActive = flag;
        }

        private string projectName;

        /// <summary>
        /// 
        /// </summary>
        public string ProjectName
        {
            get => projectName;
            set
            {
                projectName = value;
                OnPropertyChanged("ProjectName");
            }

        }

        /// <summary>
        ///
        /// </summary>
        public static BaseCommand InitCommand;

        /// <summary>
        /// 
        /// </summary>
        private int projectLen;
        /// <summary>
        /// 
        /// </summary>

        public int ProjectLen
        {
            get => projectLen;
            set
            {
                projectLen = value;
                OnPropertyChanged("ProjectLen");
            }
        }

        /// <summary>
        ///
        /// </summary>
        public FourPeopleViewModel()
        {
            if (isLogoActive)
            {
                IsLogo = Visibility.Visible;
            }
            else
            {
                IsLogo = Visibility.Collapsed;
            }

            ProjectLen = lens;
            ProjectName = projects;
            InitCommand = new BaseCommand(o => { InitStudentCommand(o); });
        }

        /// <summary>
        ///初始化
        /// </summary>
        /// <param name="o"></param>
        private void InitStudentCommand(object o)
        {
            List<Students> students = o as List<Students>;
            if (students is not null)
            {
                if (this.students.Count > 0)
                {
                    this.students.Clear();
                }
                foreach (var item in students)
                {
                    this.students.Add(item);
                }
            }
        }

        private static string projects;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Project"></param>
        public static void SetProjects(string Project)
        {
            projects = Project;
        }

        private static int lens;

        public static void SetTitleLen(int len)
        {
            lens = len;
        }
    }
}