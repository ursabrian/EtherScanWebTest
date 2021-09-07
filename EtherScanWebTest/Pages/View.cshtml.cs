

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;



using System;

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace EtherScanWebTest.Pages
{
    public class ViewModel : PageModel
    {

        private readonly ILogger<ViewModel> _logger;
        public string Message { get; private set; } = "PageModel in C#";

        public string NewComment { get; set; }


        public ViewModel(ILogger<ViewModel> logger)
        {
            _logger = logger;
        }
        public string graphContstuctor { get; set; }
        public void OnGet()
        {

           

            Message += $" Server time is { DateTime.Now }";
    

        }



        public async Task OnPostPrint()
        {
            Message += " Button Pressed";
        }
        public async Task OnPostChange()
        {
   
        
        }



    




        [BindProperty]
        public string Name { get; set; }
        [BindProperty]
        public string Email { get; set; }

        public List<string> ChartValue { get; set; } 

        public void OnPost()
        {
             ViewData["confirmation"] = $"{Name}, information will be sent to {Email}";


            Message += Name ;
        }






        string Names ;
        string symbol;
        string contract_address;
        int total_supply;
        int total_holders;


       public int total_supply_percent(int tokenPercent,int totalSupply)
        {
            return tokenPercent / totalSupply;
        }



    

        public static List<T> CreateList<T>(params T[] elements)
        {
            return new List<T>(elements);
        }


    }




}

