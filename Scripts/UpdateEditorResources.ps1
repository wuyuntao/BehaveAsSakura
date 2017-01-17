$PluginsPath = "$PSScriptRoot\..\BehaveAsSakuraEditor\Assets\Plugins"
$NodeEditorPath = "$PSScriptRoot\..\Lib\Node_Editor"
$BehaveAsSakuraPath = "$PSScriptRoot\..\BehaveAsSakura\bin\Debug"
$CompilerPath = "$PSScriptRoot\..\bassc\bin\Debug"
$FlatBufferPath = "$PSScriptRoot\..\Lib\flatbuffers"

git submodule init
git submodule update

mkdir -force $PluginsPath

cp -recurse -force "$NodeEditorPath\Editor" $PluginsPath
cp -recurse -force "$NodeEditorPath\Node_Editor" $PluginsPath

mkdir -force "$PluginsPath\BehaveAsSakura\Assemblies"

cp -recurse -force "$BehaveAsSakuraPath\BehaveAsSakura.*" "$PluginsPath\BehaveAsSakura\Assemblies"
cp -recurse -force "$FlatBufferPath\FlatBuffers.*" "$PluginsPath\BehaveAsSakura\Assemblies"

pushd $CompilerPath
& ".\bassc.exe" -o "$PluginsPath\BehaveAsSakura\Assemblies\BehaveAsSakuraSerializer.cs"
popd