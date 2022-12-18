using System;
using ScraperZap.Scripts;

namespace MyProject;
class Program
{
    static void Main(string[] args)
    {
        ZapImoveis zap = new();
        VivaReal viva = new();
        KondorImoveis kondor = new();
        Habitec habitec = new();
        //zap.ScriptZap();
        //viva.ScriptViva();
        kondor.ScriptKondor();
        //habitec.ScriptHabitec();
    }

}
    





