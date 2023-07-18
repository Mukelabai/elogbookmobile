using elogbookapi.Models;
using elogbookapi.Models.API;

using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace elogbookmobile.Services
{
    public static class DBService
    {
        static SQLiteAsyncConnection db;
        public static async Task Init()
        {
            if (db != null)
                return;

            // Get an absolute path to the database file
            var databasePath = Path.Combine(FileSystem.AppDataDirectory, "ElogbookDB.db");

            db = new SQLiteAsyncConnection(databasePath);

            //create tables here
            await db.CreateTableAsync<User>();
            await db.CreateTableAsync<APIAssignment>();
            await db.CreateTableAsync<APIMentor>();
            await db.CreateTableAsync<APIHospital>();
            await db.CreateTableAsync<APISubmission>();
            await db.CreateTableAsync<APICase>();
            await db.CreateTableAsync<APIQuestion>();
            await db.CreateTableAsync<APIQuestionResponse>();
            await db.CreateTableAsync<APIGrade>();
            await db.CreateTableAsync<APIStaffSubmission>();
            await db.CreateTableAsync<APIResponses>();
            await db.CreateTableAsync<APISubmissionComment>();
            await db.CreateTableAsync<APIAssignmentQuestionResponse>();
            await db.CreateTableAsync<ElogbookAchievement>();
            await db.CreateTableAsync<APIElogbookCompetence>();

        }

        //BEGIN USER CRUD

        public static async Task AddUser(User user)
        {
            await Init();
            await db.InsertOrReplaceAsync(user);
        }
        public static async Task DeleteUser()
        {
            await Init();
            await db.DeleteAllAsync<User>();
        }

        public static async Task<User> GetUser(string username)
        {
            await Init();
            return await db.Table<User>().Where(u => u.WebUsername == username).FirstOrDefaultAsync();
        }

        public static async Task<User> GetApplicationUser()
        {
            await Init();
            return await db.Table<User>().FirstOrDefaultAsync();
        }


        //END USER CRUD
        public static async Task AddAssignment(APIAssignment assignment)
        {
            await Init();
            await db.InsertOrReplaceAsync(assignment);
        }
        public static async Task<List<APIAssignment>> GetAssignments()
        {
            await Init();
            return await db.Table<APIAssignment>().ToListAsync();
        }
        //get only assignments that have no submission
        public static async Task<List<APIAssignment>> GetAssignmentsWithoutSubmission()
        {
            await Init();
            var submissions = await GetSubmissions();
            int[] assignmentIds = (from s in submissions
                                   select s.AssignmentId).Distinct().ToArray();
            return await db.Table<APIAssignment>().Where(a => !assignmentIds.Contains(a.AssignmentId)).ToListAsync();
        }
        //hospitals
        public static async Task AddHospital(APIHospital hospital)
        {
            await Init();
            await db.InsertOrReplaceAsync(hospital);
        }
        public static async Task<List<APIHospital>> GetHospitals()
        {
            await Init();
            return await db.Table<APIHospital>().ToListAsync();
        }

        //mentors
        public static async Task AddMentor(APIMentor mentor)
        {
            await Init();
            await db.InsertOrReplaceAsync(mentor);
        }
        public static async Task<List<APIMentor>> GetMentors()
        {
            await Init();
            return await db.Table<APIMentor>().ToListAsync();
        }

        //submisions
        public static async Task AddSubmission(APISubmission submission)
        {
            await Init();
            await db.InsertOrReplaceAsync(submission);
        }
        public static async Task<List<APISubmission>> GetSubmissions()
        {
            await Init();
            return await db.Table<APISubmission>().ToListAsync();
        }

        public static async Task<APISubmission> GetSubmission(Guid uid)
        {
            await Init();
            return await db.Table<APISubmission>().Where(s => s.SubmissionIdT == uid).FirstOrDefaultAsync();
        }
        public static async Task<APIStaffSubmission> GetStaffSubmission(Guid uid)
        {
            await Init();
            return await db.Table<APIStaffSubmission>().Where(s => s.SubmissionIdT == uid).FirstOrDefaultAsync();
        }

        public static async Task<APISubmission> GetSubmissionFromStaffSubmission(Guid uid)
        {
            APIStaffSubmission s = await GetStaffSubmission(uid);
            APISubmission submission = null;
            if (s != null)
            {
                submission = new APISubmission
                {
                    SubmissionId = s.SubmissionId,
                    SubmissionIdT = s.SubmissionIdT,
                    AssignmentId = s.AssignmentId

                };
            }
            return submission;
        }

        //case patients
        public static async Task AddCase(APICase patient)
        {
            await Init();
            await db.InsertOrReplaceAsync(patient);
        }
        public static async Task AddCase(List<APICase> patients)
        {
            await Init();
            await db.InsertAllAsync(patients);
        }
        //update case
        public static async Task UpdateCase(APICase c)
        {
            await Init();
            await db.ExecuteAsync("Update APICase set Patient=?, UpdatedBy=?, UpdatedOn=? WHERE CaseUID=?", c.Patient, c.UpdatedBy, c.UpdatedOn, c.CaseUID);
        }
        public static async Task DeleteCase(APICase c)
        {
            await Init();
            await db.ExecuteAsync("Delete from APICase WHERE CaseUID=?", c.CaseUID);
        }

        public static async Task<List<APICase>> GetSubmissionCases(Guid uid)
        {
            await Init();
            return await db.Table<APICase>().Where(s => s.SubmissionIdT == uid).ToListAsync();
        }
        public static async Task<List<APICase>> GetSubmissionCases(long submissionId)
        {
            await Init();
            return await db.Table<APICase>().Where(s => s.SubmissionId == submissionId).ToListAsync();
        }
        public static async Task<APICase> GetSubmissionCase(Guid uid, string p)
        {
            await Init();
            return await db.Table<APICase>().Where(s => s.SubmissionIdT == uid && s.Patient.ToLower() == p.ToLower()).FirstOrDefaultAsync();
        }
        public static async Task<List<APICase>> GetAllCases()
        {
            await Init();
            return await db.Table<APICase>().ToListAsync();
        }

        /// <summary>
        /// Deletes all data from the database
        /// </summary>
        /// <returns></returns>
        public static async Task CleanDatabase()
        {
            await Init();
            await db.DeleteAllAsync<APIAssignment>();
            await db.DeleteAllAsync<APIHospital>();
            await db.DeleteAllAsync<APIMentor>();
            await db.DeleteAllAsync<APISubmission>();
            await db.DeleteAllAsync<APICase>();
            await db.DeleteAllAsync<APIQuestion>();
            await db.DeleteAllAsync<APIGrade>();
            await db.DeleteAllAsync<APISubmissionComment>();
            await db.DeleteAllAsync<APIElogbookCompetence>();
            await db.DeleteAllAsync<ElogbookAchievement>();
        }


        //questions
        public static async Task DeleteQuestions(int assignmentId)
        {
            await db.ExecuteAsync("delete from APIQuestion where AssignmentId=?", assignmentId);
        }
        public static async Task DeleteComptenciesForAssignment(int assignmentId)
        {
            await db.ExecuteAsync("delete from APIElogbookCompetence where AssignmentId=?", assignmentId);
        }
        public static async Task DeleteachievementsForAssignment(int assignmentId)
        {
            await db.ExecuteAsync("delete from ElogbookAchievement where AssignmentId=?", assignmentId); 
        }
        public static async Task DeleteSubmissionsForAssignment(int assignmentId)
        {
            await db.ExecuteAsync("delete from APIStaffSubmission where AssignmentId=?", assignmentId);
        }
        public static async Task AddQuestion(APIQuestion q)
        {
            await Init();
            await db.InsertOrReplaceAsync(q);
        }
        public static async Task AddElogbookAchievement(ElogbookAchievement a)
        {
            await Init();
            await db.InsertOrReplaceAsync(a);
        }
        public static async Task AddQuestion(List<APIQuestion> questions)
        {
            await Init();
            await db.InsertAllAsync(questions);
        }

        //add all comptency deifnitons

        public static async Task AddComptencies(List<APIElogbookCompetence> competences)
        {
            await Init();
            await db.InsertAllAsync(competences);
        }
        public static async Task<List<APIElogbookCompetence>> GetComptencies( int assignmentId)
        {
          
           return await db.Table<APIElogbookCompetence>().Where(c=>c.AssignmentId==assignmentId).ToListAsync();
        }
        public static async Task<List<ElogbookAchievement>> GetAchievementsForAssignment(int assignmentId)
        {

            return await db.Table<ElogbookAchievement>().Where(c => c.AssignmentId == assignmentId).ToListAsync();
        }

        //public static async Task<List<APIQuestion>> GetSubmissionQuestions(Guid submissionIdT)
        //{
        //    await Init();
        //    return await db.Table<APIQuestion>().Where(s => s.SubmissionIdT == submissionIdT).ToListAsync();
        //}
        public static async Task<List<APIQuestion>> GetSubmissionQuestions(int AssignmentId)
        {
            await Init();
            return await db.Table<APIQuestion>().Where(s => s.AssignmentId == AssignmentId).ToListAsync();
        }

        //responses
        public static async Task UpdateSubmissionCaseCount(Guid SubmissionIdT)
        {
            await db.ExecuteAsync("update APISubmission set Cases=(select count(*) from APICase where SubmissionIdT=?), UpdatedOn=?  where SubmissionIdT=?",  SubmissionIdT,DateTime.Now, SubmissionIdT);
        }
        public static async Task DeleteSubmissionCases(Guid SubmissionIdT)
        {
            await db.ExecuteAsync("delete from APICase where SubmissionIdT=?", SubmissionIdT);
        }
        public static async Task DeleteSubmission(Guid SubmissionIdT)
        {
            //submision
            await db.ExecuteAsync("delete from APISubmission where SubmissionIdT=?", SubmissionIdT);
            //cases
            await DeleteSubmissionCases(SubmissionIdT);
            //responses
            await DeleteSubmissionResponses(SubmissionIdT);
            //general question resposes
            await DeleteAllAssignmentResponses(SubmissionIdT);
        }
        public static async Task DeleteSubmissionCases(int SubmissionId)
        {
            await db.ExecuteAsync("delete from APICase where SubmissionId=?", SubmissionId);
        }
        public static async Task DeleteSubmissionResponses(Guid SubmissionIdT)
        {
            await db.ExecuteAsync("delete from APIQuestionResponse where SubmissionIdT=?", SubmissionIdT);
        }
        public static async Task DeleteCaseResponses(Guid CaseUID)
        {
            await db.ExecuteAsync("delete from APIQuestionResponse where CaseUID=?", CaseUID);
        }
        public static async Task DeleteAPIResponses(long caseId)
        {
            await db.ExecuteAsync("delete from APIResponses where CaseId=?", caseId);
        }
        public static async Task AddResponses(APIQuestionResponse q)
        {
            await Init();
            await db.InsertOrReplaceAsync(q);
        }
        public static async Task AddResponses(List<APIQuestionResponse> responses)
        {
            await Init();
            await db.InsertAllAsync(responses);
        }

        public static async Task<List<ElogbookAchievement>> GetSubmissionAchievements(Guid submissionIdT)
        {
            //await Init();
            return await db.Table<ElogbookAchievement>().Where(s => s.SubmissionIdT == submissionIdT).ToListAsync();
        }
        public static async Task<List<ElogbookAchievement>> GetSummarisedAchievementsForAssignment(int assignmentId)
        {
            //await Init();
            return await db.QueryAsync<ElogbookAchievement>("select QuestionId, QuestionText, QuestionOption, Sum(Achieved) as Achieved,Sum(Expected) as Expected from ElogbookAchievement where AssignmentId=? group by QuestionId,QuestionText, QuestionOption", assignmentId);
        }

        public static async Task<List<APIQuestionResponse>> GetSubmissionResponses(Guid submissionIdT)
        {
            await Init();
            return await db.Table<APIQuestionResponse>().Where(s => s.SubmissionIdT == submissionIdT).ToListAsync();
        }
        public static async Task<List<APIQuestionResponse>> GetCaseResponses(Guid caseUID)
        {
            await Init();
            return await db.Table<APIQuestionResponse>().Where(s => s.CaseUID == caseUID).ToListAsync();
        }
        public static async Task<List<APIQuestionResponse>> GetCaseResponses(Guid submissionIdT, Guid caseUID)
        {
            await Init();
            return await db.Table<APIQuestionResponse>().Where(s => s.SubmissionIdT == submissionIdT && s.CaseUID == caseUID).ToListAsync();
        }
        public static async Task<List<APIResponses>> GetCaseResponses(long submissionId, long caseId)
        {
            await Init();
            return await db.Table<APIResponses>().Where(s => s.SubmissionId == submissionId && s.CaseId == caseId).ToListAsync();
        }

        //get assignment, mentor and hospital
        public static async Task<APIAssignment> GetAssignment(int asignmentId)
        {
            await Init();
            return await db.Table<APIAssignment>().Where(a => a.AssignmentId == asignmentId).FirstOrDefaultAsync();
        }
        public static async Task<APIMentor> GetMentor(int mentorId)
        {
            await Init();
            return await db.Table<APIMentor>().Where(a => a.MentorId == mentorId).FirstOrDefaultAsync();
        }
        public static async Task<APIHospital> GetHospital(int hospitalId)
        {
            await Init();
            return await db.Table<APIHospital>().Where(a => a.HospitalId == hospitalId).FirstOrDefaultAsync();
        }

        //staff submissions
        public static async Task<List<APIStaffSubmission>> GetStaffSubmissions(int assignmentId)
        {
            await Init();
            return await db.Table<APIStaffSubmission>().Where(s => s.AssignmentId == assignmentId).ToListAsync();
        }
        public static async Task AddStaffSubmission(APIStaffSubmission submission)
        {
            await Init();
            await db.InsertOrReplaceAsync(submission);
        }
        //grades
        public static async Task AddGrade(APIGrade grade)
        {
            await Init();
            await db.InsertOrReplaceAsync(grade);
        }
        public static async Task<List<APIGrade>> GetGrades()
        {
            await Init();
           return await db.QueryAsync<APIGrade>("select distinct GradeId, GradeName from APIGrade");
        }

        //public static async Task<double> GetSubmissionAchievement(int assignmentId,Guid submisionIdT)
        //{
            
        //    return await db.ExecuteScalarAsync("select distinct GradeId, GradeName from APIGrade");
        //}

        //remove all cases for supervisor 
        public static async Task DeleteAllCases()
        {
            await Init();
            await db.DeleteAllAsync<APICase>();
        }
        //remove all responses
        public static async Task DeleteAllResponses()
        {
            await Init();
            await db.DeleteAllAsync<APIResponses>();
        }
        public static async Task DeleteAllAssignmentResonses()
        {
            await Init();
            await db.DeleteAllAsync<APIAssignmentQuestionResponse>();
        }
        public static async Task DeleteAllAssignmentResponses(Guid submissionIdT)
        {
            await Init();
            await db.ExecuteAsync("Delete from APIAssignmentQuestionResponse where SubmissionIdT=?",submissionIdT);
        }
        public static async Task<List<APIAssignmentQuestionResponse>> GetAssignmentResonses(Guid submissionIdT)
        {
            await Init();
            return await db.Table<APIAssignmentQuestionResponse>().Where(r=>r.SubmissionIdT==submissionIdT).ToListAsync();
        }
        //remove all comments
        public static async Task DeleteAllComments()
        {
            await Init();
            await db.DeleteAllAsync<APISubmissionComment>();
        }
        public static async Task DeleteAllComments(Guid submissionIdT)
        {
            await Init();
            await db.ExecuteAsync("Delete from APISubmissionComment where SubmissionIdT=?", submissionIdT);
        }
        public static async Task DeleteAllSubmissionAchievements(Guid submissionIdT)
        {
            await Init();
            await db.ExecuteAsync("Delete from ElogbookAchievement where SubmissionIdT=?", submissionIdT);
        }
        public static async Task AddCaseResponses(APIResponses responses)
        {
            await Init();
            await db.InsertOrReplaceAsync(responses);
        }
        public static async Task AddAssignmentResponses(APIAssignmentQuestionResponse responses)
        {
            await Init();
            await db.InsertOrReplaceAsync(responses);
        }
        public static async Task AddAssignmentResponses(List<APIAssignmentQuestionResponse> responses)
        {
            await Init();
            await db.InsertAllAsync(responses);
        }
        public static async Task AddSubmissionAchievements(List<ElogbookAchievement> achievements)
        {
            await Init();
            await db.InsertAllAsync(achievements);
        }
        //comments
        public static async Task AddComments(List<APISubmissionComment> comments)
        {
            await Init();
            await db.InsertAllAsync(comments);
        }
        public static async Task AddComment(APISubmissionComment comment)
        {
            await Init();
            await db.InsertAsync(comment);
        }
        public static async Task<List<APISubmissionComment>> GetAssignmentComments(int assignmentId)
        {
            await Init();
            return await db.QueryAsync<APISubmissionComment>("select * from APISubmissionComment where SubmissionId in (select SubmissionId from APIStaffSubmission where AssignmentId=?)", assignmentId);
        }
        public static async Task<List<APIResponses>> GetAssignmentResponses(int assignmentId)
        {
            await Init();
            return await db.QueryAsync<APIResponses>("select * from APIResponses where SubmissionId in (select SubmissionId from APIStaffSubmission where AssignmentId=?)", assignmentId);
        }
        public static async Task<List<APIResponses>> GetSubmissionResponses(long submissionId)
        {
            await Init();
            return await db.QueryAsync<APIResponses>("select * from APIResponses where SubmissionId =?", submissionId);
        }
        public static async Task<List<APIAssignmentQuestionResponse>> GetAssignmentGeneralResponses(int assignmentId)
        {
            await Init();
            return await db.QueryAsync<APIAssignmentQuestionResponse>("select r.*,s.SubmissionId from APIAssignmentQuestionResponse r inner join APIStaffSubmission s on r.SubmissionIdT=s.SubmissionIdT where s.AssignmentId=?", assignmentId);
        }
        public static async Task<List<APIAssignmentQuestionResponse>> GetSubmissionGeneralResponses(long submissionId)
        {
            await Init();
            return await db.QueryAsync<APIAssignmentQuestionResponse>("select r.*,s.SubmissionId from APIAssignmentQuestionResponse r inner join APIStaffSubmission s on r.SubmissionIdT=s.SubmissionIdT where s.SubmissionId=?", submissionId);
        }
        public static async Task DeleteComment(APISubmissionComment comment)
        {
            await Init();
            await db.ExecuteAsync("delete from APISubmissionComment where SubmissionId=? and CommentText=?",comment.SubmissionId,comment.CommentText);
        }
        public static async Task UpdateSubmissionStatus(long submissionId, string status)
        {
            await Init();
            await db.ExecuteAsync("update APIStaffSubmission set Status=? where SubmissionId=?", status,submissionId);
        }
        public static async Task UpdateSubmissionGrade(APIStaffSubmission s)
        {
            await Init();
            await db.ExecuteAsync("update APIStaffSubmission set GradeId=?, Grade=?, GradedOn=?, GradedBy=? where SubmissionId=?", s.GradeId,s.Grade,s.GradedOn,s.GradedBy,s.SubmissionId);
        }
        //get comments for a submission
        public static async Task<List<APISubmissionComment>> GetSubmissionComments(long submissionId)
        {
            await Init();
            return await db.QueryAsync<APISubmissionComment>("Select * from APISubmissionComment where SubmissionId=? order by CreatedOn desc", submissionId);
        }

        //get comments for a submission using GUid..this is the case for students
        public static async Task<List<APISubmissionComment>> GetSubmissionComments(Guid submissionIdT)
        {
            await Init();
            return await db.QueryAsync<APISubmissionComment>("Select * from APISubmissionComment where SubmissionIdT=? order by CreatedOn desc", submissionIdT);
        }
        //delete all comments for submission
        public static async Task DeleteSubmissionComments(long SubmissionId)
        {
            await db.ExecuteAsync("delete from APISubmissionComment where SubmissionId=?", SubmissionId);
        }
    }

}
