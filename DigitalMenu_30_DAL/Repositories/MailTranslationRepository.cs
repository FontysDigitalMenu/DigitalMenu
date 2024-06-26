﻿using DigitalMenu_20_BLL.Interfaces.Repositories;
using DigitalMenu_20_BLL.Models;
using DigitalMenu_30_DAL.Data;

namespace DigitalMenu_30_DAL.Repositories;

public class MailTranslationRepository(ApplicationDbContext dbContext) : IMailTranslationRepository
{
    public MailTranslation GetMailTranslationBy(string type, string language)
    {
        MailTranslation? mailTranslation = dbContext.MailTranslations
            .FirstOrDefault(mt => mt.Type == type && mt.Language == language);

        return mailTranslation ?? dbContext.MailTranslations.First(mt => mt.Type == type && mt.Language == "en");
    }
}