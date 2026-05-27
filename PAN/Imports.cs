// .NET MAUI Toolkit
global using CommunityToolkit.Maui;
global using CommunityToolkit.Maui.Behaviors;
global using CommunityToolkit.Maui.Converters;
global using CommunityToolkit.Maui.Views;

// MVVM Toolkit
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.Messaging;

global using PAN;
global using PAN.Controls;
global using PAN.Models;
global using PAN.Services;
global using PAN.ViewModels;
global using PAN.Views;

global using VijayAnand.MauiToolkit;

// .NET MAUI Markup
global using CommunityToolkit.Maui.Markup;

// Static
global using static CommunityToolkit.Maui.Markup.GridRowsColumns;
global using static Microsoft.Maui.Graphics.Colors;

// Implicit Namespace option
// To enable, uncomment the below two lines.
//[assembly: System.Runtime.Versioning.RequiresPreviewFeatures]
//[assembly: Microsoft.Maui.Controls.Xaml.Internals.AllowImplicitXmlnsDeclaration]
// Alternatively, this can be done in the project file also.
// Set the EnablePreviewFeatures node and assign its value to true.
// And then define this constant: MauiAllowImplicitXmlnsDeclaration

// CLR Namespaces
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "PAN")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "PAN.Controls")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "PAN.Models")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "PAN.ViewModels")]
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "PAN.Views")]
// XAML Namespaces
[assembly: XmlnsDefinition("http://schemas.microsoft.com/dotnet/maui/global", "http://schemas.microsoft.com/dotnet/2022/maui/toolkit")]
