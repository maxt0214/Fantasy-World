﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameServer
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ExtremeWorldEntities : DbContext
    {
        public ExtremeWorldEntities()
            : base("name=ExtremeWorldEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<TUser> Users { get; set; }
        public virtual DbSet<TPlayer> Players { get; set; }
        public virtual DbSet<TCharacter> Characters { get; set; }
        public virtual DbSet<TCharacterItem> CharacterItems { get; set; }
        public virtual DbSet<TCharacterBag> CharacterBags { get; set; }
        public virtual DbSet<TCharacterQuest> CharacterQuests { get; set; }
        public virtual DbSet<TCharacterFriend> CharacterFriends { get; set; }
    }
}
