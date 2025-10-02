using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace studeehub.Application.DTOs.Requests.Document
{
	public class UpdateDocumentRequest
	{
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
    }
}
