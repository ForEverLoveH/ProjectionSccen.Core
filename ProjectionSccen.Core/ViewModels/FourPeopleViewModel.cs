using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using ProjectionSccen.Core.Models;

namespace ProjectionSccen.Core.ViewModels;

public class FourPeopleViewModel:ViewModelBase
{
    /// <summary>
    ///
    /// </summary>
    private ObservableCollection<StudentData> studentDatas = new ObservableCollection<StudentData>();

    /// <summary>
    ///
    /// </summary>
    public ObservableCollection<StudentData> StudentDatas
    {
        get { return studentDatas; }
        set { studentDatas = value; OnPropertyChanged("StudentDatas"); }
    }

    /// <summary>
    ///
    /// </summary>
    private Visibility isLogo;

    public Visibility IsLogo
    {
        get { return isLogo; }
        set { isLogo = value; OnPropertyChanged("IsLogo"); }
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
        if (isLogoActive) { IsLogo = Visibility.Visible; } else { IsLogo = Visibility.Collapsed; }
        ProjectLen = lens;
        ProjectName = projects;
        InitCommand = new BaseCommand(o =>
        {
            InitStudentCommand(o);
        });
    }
    /// <summary>
    ///初始化
    /// </summary>
    /// <param name="o"></param>
    private void InitStudentCommand(object o)
    {
        List<StudentData> student = o as List<StudentData>;
        if (student != null && student.Count > 0)
        {
            StudentDatas.Clear();
            for (int i = 0; i < student.Count; i++)
            {
                StudentData studentData = new StudentData()
                {
                   
                    Name = student[i].Name,
                    Score = string.IsNullOrEmpty(student[i].Score) == true ? "0" : student[i].Score,
                };
                StudentDatas.Add(studentData);
            }
        }
    }

    private static string projects;
    public static void SetProjects(string Project)
    {
        projects = Project;
    }
    private static int lens;
    public static  void  SetTitleLen(int len)
    {
        lens = len;
    }
}