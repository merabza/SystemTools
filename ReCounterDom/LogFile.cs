using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ReCounterDom;

public sealed class LogFile
{
    private readonly bool _autoCrateFolder;
    private readonly string _fileName;
    private readonly object _syncObject = new();
    private readonly bool _withCounter;
    private int _errorLogId;
    private DateTime _lastLogSaveDate = DateTime.MinValue;

    public LogFile(string fileName, bool autoCrateFolder = false, bool withCounter = false)
    {
        _withCounter = false;
        _autoCrateFolder = false;
        _withCounter = withCounter;
        _autoCrateFolder = autoCrateFolder;
        _fileName = fileName;
    }

    public void SaveLogToFile(string strLog)
    {
        string? rename = null;
        var checkFile = new FileInfo(_fileName);
        if (checkFile.Directory != null && _autoCrateFolder && !checkFile.Directory.Exists)
            checkFile.Directory.Create();
        if (checkFile.Directory is null || !checkFile.Directory.Exists)
            return;

        //შევამოწმოთ ახალი დღე ხომ არ დაიწყო
        var checkDate = DateTime.Now;
        if (_lastLogSaveDate != DateTime.MinValue && _lastLogSaveDate.Date != checkDate.Date)
            //შევამოწმოთ მოესწრო თუ არა ლოგების ფაილის შექმნა
            if (checkFile.Exists)
            {
                //ფაილის სახელის ნაწილი თარიღის გარეშე
                var fileNameWithoutDate = Path.GetFileNameWithoutExtension(_fileName) + "_";
                //დავადგინოთ ფაილის გაფართოება
                var fileExtension = Path.GetExtension(_fileName);
                var dirName = Path.GetDirectoryName(_fileName);
                if (dirName is null)
                    return;
                //დავადგინოთ არქივის ფაილის სახელი სადაც მოხდება წინა დღის ლოგების გადანახვა.
                rename = Path.Combine(dirName,
                    fileNameWithoutDate + _lastLogSaveDate.ToString("yyyyMMdd") + fileExtension);
                //შევამოწმოთ ხომ არ არსებობს ერთ თვეზე მეტი ხნის ლოგები და თუ არსებობს წავშალოთ
                foreach (var fileInfo in checkFile.Directory.GetFiles(fileNameWithoutDate + "????????" + fileExtension)
                             .Where(c => c.CreationTime.AddMonths(1) < checkDate))
                    fileInfo.Delete(); //ნაპოვნი ძველი ფაილის წაშლა
            }

        try
        {
            lock
                (_syncObject) //ამ ბრძანების გამოყენებით მივაღწიეთ იმას, რომ სხვადასხვა ნაკადი ერთდროულად არ მიმართავს ჩასაწერად ფაილს
            {
                //თუ განსაზღვრულია არქივის ფაილის სახელი, მოხდეს მიმდინარე ლოგის ფაილის გადარქმევა
                if (rename != null)
                    File.Move(_fileName, rename);

                // ReSharper disable once using
                // ReSharper disable once DisposableConstructor
                using var sw = new StreamWriter(_fileName, true, Encoding.UTF8);
                try
                {
                    sw.WriteLine(LogWithAdditionalInfo(strLog));
                }
                catch (Exception)
                {
                    // ignored
                }
                finally
                {
                    sw.Flush();
                    sw.Close();
                }
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            _lastLogSaveDate = DateTime.Now;
        }
    }

    private string LogWithAdditionalInfo(string strLog)
    {
        if (!_withCounter)
            return strLog;
        _errorLogId += 1;
        return _errorLogId + ": " + DateTime.Now + "\t" + " - " + strLog;
    }
}