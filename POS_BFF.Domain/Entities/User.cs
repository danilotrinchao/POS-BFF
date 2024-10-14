﻿using POS_BFF.Domain.Enums;
using POS_BFF.Domain.ValueObjects;

namespace POS_BFF.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string  Sobrenome{ get; set; }

        public DateTime DtNascimento { get; set; }
        public string Email { get; set; }
        public string CPF { get; set; }
        public  string Phone { get; set; }
        public EUserType UserType { get; set; }
        public Address? Address { get; set; }
        public string PasswordHash { get; set; }
        public List<Guid> RoleIds { get; set; }
        public bool Inative { get; set; }    

    }
}
