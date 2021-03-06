using CemexDictionaryApp.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Drawing;
using CemexDictionaryApp.ViewModels;

namespace CemexDictionaryApp.Repositories
{
    public class CustomerQuistionsRepository : ICustomerQuistionsRepository
    {
        DBContext context;
        private readonly IWebHostEnvironment hostEnvironment;
        public CustomerQuistionsRepository(DBContext _context, IWebHostEnvironment _hostEnvironment)
        {
            context = _context;
            hostEnvironment = _hostEnvironment;

        }

        public List<CustomerQuestions> GetAll()
        {
            List<CustomerQuestions> Questions = context.customer_Questions.Include(question => question.QuestionMedia).
                ToList();
            return Questions;
        }

        public List<CustomerQuestions> GetAllByCategoryId(int _categoryId)
        {
            List<CustomerQuestions> Questions = context.customer_Questions
                .Include(question => question.QuestionMedia).
               Where(question => question.CategoryId == _categoryId).ToList();

            return Questions;
        }

        public List<string> UploadFile(List<string> base64Images)
        {
            List<string> images = new List<string>();
            //long size = FormFile.Sum(f => f.Length);
            int count = 0;
            foreach (var base64image in base64Images)
            {
                if (base64image != null)
                {
                    //string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath);
                    //images.Add(Guid.NewGuid().ToString() + "_" + formFile.FileName);
                    //string path = Path.Combine(uploadsFolder + @"\images\CustomerQuestions\", images[count]);
                    //count++;
                    //using (var fileStream = new FileStream(path, FileMode.Create))
                    //{
                    //    formFile.CopyTo(fileStream);
                    //}


                    string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath);
                    string imageName = Guid.NewGuid().ToString() + ".jpg";
                    images.Add(imageName);

                    string path = Path.Combine(uploadsFolder + @"\images\CustomerQuestions\", images[count]);
                    count++;

                    //Check if directory exist
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path); //Create directory if it doesn't exist
                    }

                  

                    //set the image path
                    string imgPath = Path.Combine(path, imageName);

                    byte[] imageBytes = Convert.FromBase64String(base64image);

                    File.WriteAllBytes(imgPath, imageBytes);
                }
            }
            return images;
        }




        public List<string> UploadImagesByAdmin(List<IFormFile> FormFile)
        {
            List<string> images = new List<string>();
            long size = FormFile.Sum(f => f.Length);
            int count = 0;
            foreach (var formFile in FormFile)
            {
                if (formFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath);
                    images.Add(Guid.NewGuid().ToString() + "_" + formFile.FileName);
                    string path = Path.Combine(uploadsFolder + @"\images\Questions\", images[count]);
                    count++;
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        formFile.CopyTo(fileStream);
                    }
                }
            }
            return images;
        }


        public int Insert(CustomerQuestions question)
        {
            context.customer_Questions.Add(question);
            return context.SaveChanges();
        }

        public CustomerQuestions GetById(int QuestionId)
        {
            CustomerQuestions question = context.customer_Questions.Include(question => question.QuestionMedia).
                FirstOrDefault(question => question.ID == QuestionId);
            return question;
        }

        public int AnswerQuestion(int QuestionId, CustomerQuestions questionWithAnswer)
        {
            CustomerQuestions question = context.customer_Questions.Include(question => question.QuestionMedia).
            FirstOrDefault(question => question.ID == QuestionId);
            question.Answer = questionWithAnswer.Answer;
            question.Status = Question_Status.Answered.ToString();
            question.IsRead = true;
            question.AdminId = questionWithAnswer.AdminId;
            context.customer_Questions.Update(question);
            context.SaveChanges();

            return question.ID;
        }
        public int RejectQuestion(int QuestionId, string comment)
        {
            CustomerQuestions question = context.customer_Questions.Include(question => question.QuestionMedia).
            FirstOrDefault(question => question.ID == QuestionId);

            if (comment != null)
            {
                question.Comment = comment;
            }

            question.Status = Question_Status.Rejected.ToString();
            context.customer_Questions.Update(question);
            context.SaveChanges();
            return question.ID;
        }
    }
}
