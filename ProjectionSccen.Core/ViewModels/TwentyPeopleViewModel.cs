using ProjectionSccen.Core.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows;
using System.Collections.Generic;

namespace ProjectionSccen.Core.ViewModels;

public class TwentyPeopleViewModel:ViewModelBase
{
    /// <summary>
    /// 
    /// </summary>
    private int titleLen;
    /// <summary>
    /// 
    /// </summary>
    public int TitleLen
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
    public  static  void  SetProjects(string project)
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
    public static BaseCommand InitCommand;
    /// <summary>
    ///
    /// </summary>
    public TwentyPeopleViewModel()
    {

        projectName =string.IsNullOrEmpty(projects)==true?null:projects;
        titleLen = title;
        InitCommand = new BaseCommand(o =>
        {
            InitStudentCommand(o);
        });
    }
    /// <summary>
    ///
    /// </summary>
    /// <param name="obj"></param>
    private void InitStudentCommand(object obj)
    {
        List<StudentData> student = obj as List<StudentData>;
        if (student != null && student.Count > 0)
        {
            StudentDatas.Clear();
            for (int i = 0; i < student.Count; i++)
            {
                StudentData studentData = new StudentData()
                {
                    
                    Name = student[i].Name,
                    School = student[i].School,
                    Score = string.IsNullOrEmpty(student[i].Score) == true ? "0" : student[i].Score,
                };
                StudentDatas.Add(studentData);
            }
        }
    }
}