using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ImageGallery.Client.ViewModels
{
    public class OrderFrameViewModel
    {
        public AddressViewModel Address { get; private set; } = null;

        public OrderFrameViewModel(string address)
        {
            Address = JsonConvert.DeserializeObject<AddressViewModel>(address);
        }
    }

}
