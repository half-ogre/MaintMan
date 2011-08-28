using System.IO;
using System.IO.Compression;
using System.Text;
using tar_cs;

namespace MaintMan
{
    public class MaintenanceTarballCreator : IMaintenanceTarballCreator
    {
        public byte[] Create(string maintenanceHtml)
        {
            MemoryStream tarGzStream;
            
            using (var input = new MemoryStream(Encoding.UTF8.GetBytes(maintenanceHtml), false))
            using (tarGzStream = new MemoryStream())
            using (var gzStream = new GZipStream(tarGzStream, CompressionMode.Compress))
            using (var tarWriter = new TarWriter(gzStream))
            {
                tarWriter.Write(input, input.Length, "App_Offline.htm");
            }

            return tarGzStream.GetBuffer();
        }
    }
}