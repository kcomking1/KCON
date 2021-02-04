using System; 

namespace KCSystem.Core.Entities
{
    public class RolePower:Entity
    {
        /// <summary>
        /// 权限名称
        /// </summary>
        public string PowerName { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        public Role Role { get; set; }

    }
}
