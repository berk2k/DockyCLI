namespace Docky.Core.Models
{
    public class ContainerCreateParams
    {
        public string ImageName { get; set; } = string.Empty;
        public string ContainerName { get; set; } = string.Empty;
        public string Ports { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public string Volumes { get; set; } = string.Empty;
        public string AdditionalParams { get; set; } = string.Empty;
        public bool DetachedMode { get; set; } = true;
        public bool InteractiveMode { get; set; } = false;
        public bool RemoveOnExit { get; set; } = false;
    }
}