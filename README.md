# FlexNet

> A Networking Libary that is simple to use, but Flexible by Extensibility

[![Build status](https://ci.appveyor.com/api/projects/status/mm9a0a7yxhuw7t0n/branch/master?svg=true)](https://ci.appveyor.com/project/HurricanKai/flexnet/branch/master)

## Table of Contents

- [Install](#install)
- [Usage](#usage)
- [API](#api)
- [Contributing](#contributing)
- [License](#license)

## Install
Install using NuGet:

```
// TODO: Command to install
```

## Usage
To Get Started, a Protocol has to be defined.
This is done using the ProtocolBuilder, the Below Code Creates a new ProtocolDefinition, using int as IdType, and Registers one Packet, `SingleByteTransferPacket`, using Id 0x00.
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

For more Information check the [Samples](./Samples/)

## API
To extend FlexNet, you can implement one of the following, depending on your needs:

[INetworkAccessor](./FlexNet.Core/INetworkAccessor.cs) defining how a Type should be Serialized. see [DefaultAccessors](./FlexNet.Core.DefaultAccessors) for Samples & the Default Accessors.
[ILengthBehaviour](./FlexNet.Core/ILengthBehaviour.cs) defining how Length should be Serialized. [This Folder](./FlexNet.Core/LengthBehaviours) Contains the Default Provided Length Behaviours
[IIdHeader](./FlexNet.Core/IIdHeader.cs) defining how Id should be Serialized. [DefaultIdHeader](./FlexNet.Core/DefaultIdHeader.cs) will most of the Time be enough.
[IIdMapper](./FlexNet.Core/IIdMapper.cs) defining how Ids should be Mapped to Packets, Offically only a Generic [DelegateIdMapper](./FlexNet.Core/DelegateIdMapper.cs) is Provided.
[IClient](./FlexNet.Core/IClient.cs) defining Client Functionality.Currently [SimpleTCP](./Templates/SimpleTCP/TcpClient.cs) is the only Provided.
[IServer](./FlexNet.Core/IServer.cs) defining Server Functionality. Currently [SimpleTCP](./Templates/SimpleTCP/TcpServer.cs) is the only Provided.
[IDelegateBuilder](./FlexNet.Core/IDelegateBuilder.cs) defining how the Read and Write Delegates are Build. These are Shipped in theire own Packages, the Current Offical ones are:
    [ExpressionDelegateBuilder](./Builders/ExpressionDelegateBuilder)


## Building
To Build this Project you will need to be able to run a Cake Build Script.
Additionally .Net Core 2.1 will be needed.

All Tests Target .Net Core 2.1, and all Libary parts target .Net Standard 2.0

## Contributing

See [the contributing file](CONTRIBUTING.md)!

PRs are very welcome!

## License

[MIT © Kai Jellinghaus](./LICENSE)
