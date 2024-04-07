using ProjectionSccen.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace ProjectionSccen.Core.ViewModels{

    public class TwoPeopleViewModel : ViewModelBase
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
        private Visibility isLogo = Visibility.Visible;
        /// <summary>
        /// 
        /// </summary>
        public Visibility IsLogo
        {
            get => isLogo;
            set
            {
                isLogo = value;
                OnPropertyChanged("IsLogo");
            }
        }

        /// <summary>
        ///
        /// </summary>
        private static bool isLogoActive= true;

        /// <summary>
        ///
        /// </summary>
        /// <param name="flag"></param>
        public static void SetLogo(bool flag)
        {
            isLogoActive = flag;
        }

        /// <summary>
        /// 
        /// </summary>
        private int tiTileLen;

        /// <summary>
        /// 
        /// </summary>
        public int TiTileLen
        {
            get => tiTileLen;
            set
            {
                tiTileLen = value;
                OnPropertyChanged("TiTileLen");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private int examTime;

        /// <summary>
        /// 
        /// </summary>
        public int ExamTime
        {
            get => examTime;
            set
            {
                tiTileLen = value;
                OnPropertyChanged("ExamTime");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static int len;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lens"></param>
        public static void SetLen(int lens)
        {
            len = lens;
        }

        /// <summary>
        ///
        /// </summary>
        public static BaseCommand InitCommand;

        /// <summary>
        ///
        /// </summary>
        public TwoPeopleViewModel()
        {
            if (isLogoActive) isLogo = Visibility.Visible;
            else
            {
                isLogo = Visibility.Hidden;
            }

            tiTileLen = len;
            InitCommand = new BaseCommand(o => { InitStudentCommand(o); });
        }

        /// <summary>
        ///初始化
        /// </summary>
        /// <param name="o"></param>
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