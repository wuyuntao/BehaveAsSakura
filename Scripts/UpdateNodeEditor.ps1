$PluginsPath = "$PSScriptRoot\..\BehaveAsSakuraEditor\Assets\Plugins"
$NodeEditorPath = "$PSScriptRoot\..\Lib\Node_Editor"

git submodule init
git submodule update

mkdir $PluginsPath

cp -recurse -force "$NodeEditorPath\Editor" $PluginsPath
cp -recurse -force "$NodeEditorPath\Node_Editor" $PluginsPath