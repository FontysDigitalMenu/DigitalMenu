﻿namespace DigitalMenu_20_BLL.Services;

public static class DateTimeService
{
    public static DateTime GetNow()
    {
        return DateTime.Now; //DateTime.Today.AddHours(18).AddMinutes(0);;
    }
}