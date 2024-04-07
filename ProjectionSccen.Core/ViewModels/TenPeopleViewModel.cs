using ProjectionSccen.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ProjectionSccen.Core.ViewModels{

    public class TenPeopleViewModel : ViewModelBase
    {
        private string projectName { get; set; }

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

        private int len;

        public int ProjectLen
        {
            get => len;
            set
            {
                len = value;
                OnPropertyChanged("ProjectLen");
            }
        }

        private static int Len;

        public static void SetProjectLen(int len)
        {
            Len = len;
        }

        /// <summary>
        ///
        /// </summary>
        private ObservableCollection<Students> students { get; set; } = new ObservableCollection<Students>();

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

        private static string project;

        /// <summary>
        ///
        /// </summary>
        /// <param name="projectName"></param>
        public static void SetProjectName(string projectName)
        {
            project = projectName;
        }

        /// <summary>
        ///
        /// </summary>
        public static BaseCommand InitCommand { get; set; }

        /// <summary>
        ///
        /// </summary>
        public TenPeopleViewModel()
        {

            len = Len;
            ProjectName = string.IsNullOrEmpty(project) == true ? string.Empty : project;
            InitCommand = new BaseCommand(obj => InitUserData(obj));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        private void InitUserData(object obj)
        {
            List<Students> students = obj as List<Students>;
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
    }
}