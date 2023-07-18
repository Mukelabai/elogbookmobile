using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for ElogbookAchievement
/// </summary>
public class ElogbookAchievement
{
    public ElogbookAchievement()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public int QuestionId { get; set; }
    public string QuestionOption { get; set; }
    public string ResponseText { get; set; }
    public int StudentId { get; set; }
    public int InstitutionId { get; set; }
    public long SubmissionId { get; set; }
    public Guid SubmissionIdT { get; set; }
    public int ElogbookId { get; set; }
    public int AssignmentId { get; set; }
    public int Expected { get; set; }
    public int Achieved { get; set; }
    public string QuestionText { get; set; }
    public double AchievedPercentage { get; set; }
}