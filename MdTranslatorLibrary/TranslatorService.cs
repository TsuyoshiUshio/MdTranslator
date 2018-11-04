using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MdTranslatorLibrary
{
    public interface ITranslatorService
    {
        Task<string> TranslateAsync(string text, string language);
    }
    public class TranslatorService
    {
        private ITranslatorRepository repository;
        public TranslatorService(ITranslatorRepository repository)
        {
            this.repository = repository;
        }

        public Task<string> TranslateAsync(string text, string language)
        {
            return repository.TranslateAsync(text, language);
        }
    }
}
