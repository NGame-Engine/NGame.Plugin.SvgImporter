using System.Drawing.Imaging;
using Microsoft.Build.Framework;
using Svg;
using Task = Microsoft.Build.Utilities.Task;

namespace NGame.Plugin.SvgImporter;



public class WriteSvgToPng : Task
{
	[Required] public ITaskItem[] SvgAssets { get; set; } = null!;
	[Required] public string TargetFolder { get; set; } = null!;

	[Output] public ITaskItem[]? ChangedAssets { get; set; }


	public override bool Execute()
	{
		try
		{
			foreach (var svgAsset in SvgAssets)
			{
				var oldDataFilePath = svgAsset.GetMetadata("DataFile");

				var newRelativeFilePath = Path.ChangeExtension(oldDataFilePath, ".png");
				var newDataFilePath = Path.Combine(TargetFolder, newRelativeFilePath);

				var newFileDirectoryName = Path.GetDirectoryName(newDataFilePath)!;
				Directory.CreateDirectory(newFileDirectoryName);

				var svgDocument = SvgDocument.Open<SvgDocument>(oldDataFilePath);
				var bitmap = svgDocument.Draw();
				bitmap.Save(newDataFilePath, ImageFormat.Png);

				svgAsset.SetMetadata("DataFile", newDataFilePath);
				svgAsset.SetMetadata("DataFileExtension", ".png");
			}

			ChangedAssets = SvgAssets;
		}
		catch (Exception e)
		{
			Log.LogErrorFromException(e, true);
		}

		return Log.HasLoggedErrors == false;
	}
}
