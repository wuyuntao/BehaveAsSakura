$PluginsPath = "$PSScriptRoot\..\BehaveAsSakuraEditor\Assets\Plugins"
$NodeEditorPath = "$PSScriptRoot\..\Lib\Node_Editor"
$BehaveAsSakuraPath = "$PSScriptRoot\..\BehaveAsSakura\bin\Debug"
$CompilerPath = "$PSScriptRoot\..\bassc\bin\Debug"
$FlatBufferPath = "$PSScriptRoot\..\Lib\flatbuffers"

git submodule init
git submodule update

mkdir -force $PluginsPath

mkdir -force "$PluginsPath\BehaveAsSakuraEditor\Assemblies"

cp -recurse -force "$BehaveAsSakuraPath\BehaveAsSakura.*" "$PluginsPath\BehaveAsSakuraEditor\Assemblies"
cp -recurse -force "$FlatBufferPath\FlatBuffers.*" "$PluginsPath\BehaveAsSakuraEditor\Assemblies"

pushd $CompilerPath
& ".\bassc.exe" -o "$PluginsPath\BehaveAsSakuraEditor\Assemblies\BehaveAsSakuraSerializer.cs"
popd