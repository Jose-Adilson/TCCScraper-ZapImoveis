using System;
using ScraperZap.Scripts;
using ScraperZap.Shared;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        ZapImoveis zap = new();
        VivaReal viva = new();
        KondorImoveis kondor = new();
        Habitec habitec = new();
        dbmaintain dbmaintain = new();
        zap.ScriptZap(1, 1);
        viva.ScriptViva(1, 1);
        kondor.ScriptKondor(1);
        habitec.ScriptHabitec(1, 1);
        dbmaintain.MainForm();

    }

}
    





