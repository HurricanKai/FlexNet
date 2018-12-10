# FlexNet

> A Networking Libary that is simple to use, but Flexible by Extensibility

[![Build status](https://ci.appveyor.com/api/projects/status/mm9a0a7yxhuw7t0n/branch/master?svg=true)](https://ci.appveyor.com/project/HurricanKai/flexnet/branch/master)

## Table of Contents

- [Downloads](#downloads)
- [Usage](#usage)
- [API](#api)
- [Contributing](#contributing)
- [License](#license)

## Downloads
All Packets are Aviable on Nuget, or in the [Release Section](https://github.com/HurricanKai/FlexNet/releases)
Below is a Table of all Nuget Packets

Name | Package Manager
:--- | :---
[Core](https://www.nuget.org/packages/FlexNet.Core/) | `Install-Package FlexNet.Core`
[Default Accessors](https://www.nuget.org/packages/FlexNet.Core.DefaultAccessors/) | `Install-Package FlexNet.Core.DefaultAccessors`
[SimpleTCP Template](https://www.nuget.org/packages/FlexNet.Templates.SimpleTCP/) | `Install-Package FlexNet.Templates.SimpleTCP`
[ExpressionDelegate Builder](https://www.nuget.org/packages/FlexNet.Builders.ExpressionDelegateBuilder/) | `Install-Package FlexNet.Builders.ExpressionDelegateBuilder`

## Usage
To get started, a `Protocol` has to be defined.
This is done using the `ProtocolBuilder`.
The below code creates a new `ProtocolDefinition`, using `int` as the `IdType`, and registers one `Packet`, `SingleByteTransferPacket`, with `0x00` as the ID.
```c#
ProtocolBuilder
    .Create<int>()
    .LengthBehaviour(new DefaultDynamicInt32LengthBehaviour())
    .UseIds(new DefaultIdHeader(), 
    new DelegateIdResolver((id, protocol) 
        => protocol.Packets.First(x => ((int)x.Id) == (int)id)))
    .RegisterDefaultAccessors()
    .RegisterPacket((packetBuilder) => packetBuilder
        .Id(0x00)
        .BindingType<SingleByteTransferPacket>()
        .BindField(nameof(SingleByteTransferPacket.Data))
    ).Build(new ExpressionDelegateBuilder());
```

For more information, check the [Samples](./Samples/)

## API
To extend FlexNet, you can implement one of the following, depending on your needs:

Interface | Defines | Default Implementation
:--- | :--- | ---:
[INetworkAccessor](./FlexNet.Core/INetworkAccessor.cs) | How a `Type` should be serialized. | [DefaultAccessors](./FlexNet.Core.DefaultAccessors)
[ILengthBehaviour](./FlexNet.Core/ILengthBehaviour.cs) | How `Length` should be serialized. | [This Folder](./FlexNet.Core/LengthBehaviours)
[IIdHeader](./FlexNet.Core/IIdHeader.cs) | How an `Id` should be serialized. | [DefaultIdHeader](./FlexNet.Core/DefaultIdHeader.cs)
[IIdMapper](./FlexNet.Core/IIdMapper.cs) | How `Id`s should be mapped to `Packet`s | [DelegateIdMapper](./FlexNet.Core/DelegateIdMapper.cs)
[IClient](./FlexNet.Core/IClient.cs) | Client Functionality | [SimpleTCP Client](./Templates/SimpleTCP/TcpClient.cs)
[IServer](./FlexNet.Core/IServer.cs) | Server Functionality. | [SimpleTCP Server](./Templates/SimpleTCP/TcpServer.cs)
[IDelegateBuilder](./FlexNet.Core/IDelegateBuilder.cs) | How the read/write delegates are built. | [ExpressionDelegateBuilder](./Builders/ExpressionDelegateBuilder)

## Building
To build this project, you will need to be able to run a `Cake Build Script`.
Additionally, `.NET Core 2.1` will be needed.

All unit tests target `.NET Core 2.1`, and `FlexNet` targets `.NET Standard 2.0`

## Contributing

See [the contributing file](CONTRIBUTING.md)!

PRs are very welcome!

## License

[MIT © Kai Jellinghaus](./LICENSE)
