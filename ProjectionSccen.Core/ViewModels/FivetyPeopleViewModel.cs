using ProjectionSccen.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProjectionSccen.Core.ViewModels{

    public class FivetyPeopleViewModel : ViewModelBase
    {
        /// <summary>
        ///
        /// </summary>
        private int titleLen;

        /// <summary>
        ///
        /// </summary>
        public int ProjectNameLen
        {
            get { return titleLen; }
            set
            {
                titleLen = value;
                OnPropertyChanged("TitleLen");
            }
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
        private static string projects;

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        public static void SetProjects(string project)
        {
            projects = project;
        }

        /// <summary>
        ///
        /// </summary>
        private static int title;

        /// <summary>
        ///
        /// </summary>
        /// <param name="titleLen"></param>
        public static void SetTitle(int titleLen)
        {
            title = titleLen;
        }

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<Students> students = new ObservableCollection<Students>();

        /// <summary>
        ///
        /// </summary>
        public ObservableCollection<Students> StudentDatas
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
        public static BaseCommand InitCommand;

        /// <summary>
        ///
        /// </summary>
        public FivetyPeopleViewModel()
        {
            projectName = string.IsNullOrEmpty(projects) == true ? null : projects;
            titleLen = title;
            InitCommand = new BaseCommand(o => { InitStudentCommand(o); });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        private void InitStudentCommand(object o)
        {
            List<Students> students = o as List<Students>;
            if (students != null)
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
    }
}