$UnityPluginDir = "$PSScriptRoot\..\BehaveAsSakuraUnity\Assets\BehaveAsSakura"
$RuntimeDir = "$PSScriptRoot\..\BehaveAsSakura\bin\Debug"
$EditorDir = "$PSScriptRoot\..\BehaveAsSakuraEditor\bin\Debug"
$CompilerDir = "$PSScriptRoot\..\bassc\bin\Debug"
$FlatBuffersDir = "$PSScriptRoot\..\Lib\flatbuffers"

cp -recurse -force "$FlatBuffersDir\FlatBuffers.*" "$UnityPluginDir\Runtime"
cp -recurse -force "$RuntimeDir\BehaveAsSakura.*" "$UnityPluginDir\Runtime"
cp -recurse -force "$EditorDir\BehaveAsSakuraEditor.*" "$UnityPluginDir\Runtime"

pushd $CompilerDir
& ".\bassc.exe" -o "$UnityPluginDir\Runtime\FlatBuffersSerializer.cs"
popd