#r "packages/FAKE/tools/FakeLib.dll" // include Fake lib
open Fake

Target "Test" (fun _ -> trace "test")

RunTargetOrDefault "Test"