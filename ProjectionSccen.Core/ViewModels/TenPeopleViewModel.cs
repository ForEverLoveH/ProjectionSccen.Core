using ProjectionSccen.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;

namespace ProjectionSccen.Core.ViewModels;

public class TenPeopleViewModel : ViewModelBase
{
    private string projectName { get; set; }

    /// <summary>
    ///
    /// </summary>
    public string ProjectName
    {
        get { return projectName; }
        set { projectName = value; OnPropertyChanged("ProjectName"); }
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
    public static void  SetProjectLen(int len)
    {
        Len= len;
    }
    /// <summary>
    ///
    /// </summary>
    private ObservableCollection<StudentData> studentDatas { get; set; } = new ObservableCollection<StudentData>();

    /// <summary>
    ///
    /// </summary>
    public ObservableCollection<StudentData> StudentDatas
    {
        get { return studentDatas; }
        set
        {
            studentDatas = value; OnPropertyChanged("StudentDatas");
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