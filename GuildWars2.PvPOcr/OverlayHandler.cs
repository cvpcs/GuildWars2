using GuildWars2.Overlay.Contract;

namespace GuildWars2.PvPOcr
{
    public static class OverlayHandler
    {
        public static void Initialize()
        {
            OverlayManager.LoadHTML(
                @"
<!DOCTYPE HTML>
<html>
  <head>
    <title></title>
    <script src=""http://ajax.aspnetcdn.com/ajax/jquery/jquery-1.9.0.js""></script>
    <script type=""text/javascript"">
function loadScores(red, blue)
{
  if (red >= 0)
  {
    $('.scoreRed').width(Math.floor((red/500) * 100) + '%');
  }
  if (blue >= 0)
  {
    $('.scoreBlue').width(Math.floor((blue/500) * 100) + '%');
  }
}
    </script>
    <style type=""text/css"">
html, body
{
  background-color: rgba(0, 0, 0, 0);
  overflow: hidden;
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  margin: 0;
  padding: 0;
}

div.scoreContainer
{
  width: 40%;
  height: 100%;
  position: relative;
  border-top-style: solid;
  border-top-width: 2px;
  border-bottom-style: solid;
  border-bottom-style: 2px;
}

div.scoreContainerRed { float: left; border-color: rgba(255, 0, 0, 0.75); }
div.scoreContainerBlue { float: right; border-color: rgba(0, 0, 255, 0.75); }

div.score
{
  height: 100%;
}

div.scoreRed { float: left; background-color: rgba(255, 0, 0, 0.75); }
div.scoreBlue { float: right; background-color: rgba(0, 0, 255, 0.75); }
    </style>
  </head>
  <body>
    <div class=""scoreContainer scoreContainerRed"">
      <div class=""score scoreRed"" style=""width: 0%"">&nbsp;</div>
    </div>
    <div class=""scoreContainer scoreContainerBlue"">
      <div class=""score scoreBlue"" style=""width: 0%"">&nbsp;</div>
    </div>
  </body>
</html>
"
                );
        }

        public static void LoadScores(int red, int blue)
        {
            OverlayManager.ExecuteJavascript(string.Format("loadScores({0}, {1});", red, blue));
        }
    }
}
