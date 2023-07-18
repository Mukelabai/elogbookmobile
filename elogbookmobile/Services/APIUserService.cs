using elogbookapi.Models;
using elogbookapi.Models.API;
using elogbookapi.Models.Students;
using elogbookmobile.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace elogbookmobile.Services
{
    public static class APIUserService
    {

        static string baseURL = "https://elogapi.coinfomas.net";
        private const string APIKeyToCheck = "86842a38-310c-48cf-a64a-98ef66c1e775";

        static HttpClient httpClient;
        static APIUserService()
        {
            httpClient = new HttpClient { BaseAddress = new Uri(baseURL) };
            httpClient.DefaultRequestHeaders.Add("APIKey", APIKeyToCheck);

        }
        public static async Task<User> IsValidUser(string username, string password)
        {
            User user = null;
            try
            {
                var json = await httpClient.GetStringAsync(string.Format("api/users/?username={0}&password={1}", username, password));
                user = JsonConvert.DeserializeObject<User>(json);

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            return user;
        }

        //get user data
        public static async Task<UserData> GetUserData(string username, bool isStudent)
        {
            UserData userData = null;
            try
            {
                var json = await httpClient.GetStringAsync(string.Format("api/users/?username={0}&isStudent={1}", username, isStudent));
                userData = JsonConvert.DeserializeObject<UserData>(json);

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            return userData;
        }

        //get questions
        public static async Task<List<APIQuestion>> GetQuestionsForSubmission(string submissionIdT)
        {
            var json = await httpClient.GetStringAsync(string.Format("api/questions/?SubmissionIdT={0}", submissionIdT));
            var questions = JsonConvert.DeserializeObject<List<APIQuestion>>(json);
            return questions;

        }
        //get submision data
        public static async Task<APISubmissionData> GetSubmissionData(string submissionIdT)
        {
            var json = await httpClient.GetStringAsync(string.Format("api/submissions/?SubmissionIdT={0}", submissionIdT));
            var data = JsonConvert.DeserializeObject<APISubmissionData>(json);
            return data;

        }
        //get staff submision data
        public static async Task<APIAssignmentData> GetAssignmentData(int assignmentId)
        {
            var json = await httpClient.GetStringAsync(string.Format("api/staffsubmission/?assignmentId={0}", assignmentId));
            var data = JsonConvert.DeserializeObject<APIAssignmentData>(json);
            return data;

        }

        //get  submision case data
        public static async Task<APICaseData> GetCaseData(string submissionIds)
        {
            var json = await httpClient.GetStringAsync(string.Format("api/staffsubmission/?submissionIds={0}", submissionIds));
            var data = JsonConvert.DeserializeObject<APICaseData>(json);
            return data;

        }

        public static async Task PostSubmissionData(APISubmissionData data)
        {
            try
            {
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/submissions", content);

                var errorData = JsonConvert.DeserializeObject<List<APIError>>(await response.Content.ReadAsStringAsync());
                string[] infoMessages = (from m in errorData.Where(e => e.MessageType == "I")
                                         select m.ErrorMessage).ToArray();
                string[] errorMessages = (from m in errorData.Where(e => e.MessageType == "E")
                                          select m.ErrorMessage).ToArray();
                await Utility.DisplayInfoMessage("Successfully synced the following:\n" + string.Join("\n", infoMessages));
                if (errorMessages.Length > 0)
                {
                    await Utility.DisplayErrorMessage(string.Join("\n", errorMessages));
                }

                //if (!response.IsSuccessStatusCode)
                //{
                //    await Utility.DisplayErrorMessage(response.ReasonPhrase);
                //}

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
        }

        public static async Task<List<APIError>> PostStaffSubmissionData(APISubmissionCommentData data)
        {
            List<APIError> messages = new List<APIError>();
            try
            {
                //data.Responses = data.Responses.Where(r => r.CaseId <= 140).ToList();
                var json = JsonConvert.SerializeObject(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/staffsubmission", content);

                var errorData = JsonConvert.DeserializeObject<List<APIError>>(await response.Content.ReadAsStringAsync());

                messages = errorData;

            }
            catch (Exception ex)
            {
                await Utility.DisplayErrorMessage(ex);
            }
            return messages;
        }

    }
}
