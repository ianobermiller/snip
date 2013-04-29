//-----------------------------------------------------------------------
// <copyright file="Snippet.cs" company="Microsoft">
//     Copyright (c) iano, Microsoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Snip.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Web.Mvc;
    using System.Xml.Serialization;

    public class Snippet
    {
        [Remote("IsAvailable")]
        public string Id { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Content { get; set; }

        public string Creator { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public int ViewCount { get; set; }

        public DateTime? Expiration { get; set; }

        [Display(Name="Expiration")]
        [XmlIgnore]
        public int ExpirationInMinutes { get; set; }

        public DateTime LastView { get; set; }

        public Language Language { get; set; }

        public Snippet()
        {
            this.Language = Language.Auto;
        }

        public string GetDisplayName()
        {
            return this.Title.OrIfEmpty(this.Id);
        }
    }
}