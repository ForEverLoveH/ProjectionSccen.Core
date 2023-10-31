using ProjectionSccen.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace ProjectionSccen.Core.ViewModels;

public class TwoPeopleViewModel : ViewModelBase
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
    /// <summary>
    /// 
    /// </summary>
    private int tiTileLen;
    /// <summary>
    /// 
    /// </summary>
    public int TiTileLen
    {
        get=>tiTileLen; set { tiTileLen = value; OnPropertyChanged("TiTileLen"); } 
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
        if (isLogoActive) { IsLogo = Visibility.Visible; } else { IsLogo = Visibility.Collapsed; }
        tiTileLen=len; 
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
                    School = student[i].School,
                    Name = student[i].Name,
                    Score = string.IsNullOrEmpty(student[i].Score) == true ? "0" : student[i].Score,
                };
                StudentDatas.Add(studentData);
            }
        }
    }
}