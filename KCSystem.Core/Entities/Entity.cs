using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;  

namespace KCSystem.Core.Entities
{
   public abstract class  Entity: BaseEntity
    {
         

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public  int Id { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool Disabled { get; set; }
        //[Timestamp]
        //public byte[] Timestamp { get; set; }


        /// <summary>
        /// 添加时间
        /// </summary> 
        public DateTime AddTime { get; set; } =DateTime.Now;

        /// <summary>
        /// 验证数据信息
        /// </summary>
        /// <returns></returns>
        public (bool State, string Message) Validate()
        {
            var validationContext = new ValidationContext(this);
            var validationResults = new List<ValidationResult>();

            var succeed = Validator.TryValidateObject(this, validationContext, validationResults, true);
            
            var message = new StringBuilder();
            if (!succeed)
            {
                
                foreach (var validationResult in validationResults)
                {
                    message.AppendLine(validationResult.ErrorMessage);
                } 
            }

            return (succeed, message.ToString());
        }
    }
}
