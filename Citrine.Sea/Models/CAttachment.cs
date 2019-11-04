#pragma warning disable CS8618 // API POCO クラスは除外
using System;
using System.Linq;
using Citrine.Core.Api;

namespace Citrine.Sea
{
    public class CAttachment : IAttachment
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public string PreviewUrl => Url;

        public DateTime CreatedAt => default;

        public string Comment => Name;

		public CAttachment(File file)
		{
			Id = file.Id.ToString();

			Name = file.Name;

			Url = file.Variants.First().Url;
		}
    }
}
