

# FlexNet

> A Networking Libary that is simple to use, but Flexible by Extensibility

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
To extend FlexNet, you can write one of the following, depending on your needs:

|Name|Description|Offically Provided|
|--|--|--|
|INetworkAccessor|Defines how a Type should be Serialized|See DefaultAccessors
|ILengthBehaviour|Defines how Length should be Serialized|DefaultDynamicInt32LengthBehaviour, ConstantLengthBehaviour in Core|
|IDelegateBuilder|Defines how Read/Write Delegates are Build|ExpressionDelegateBuilder in FlexNet.Builders.ExpressionDelegateBuilder|
|IIdHeader|Defines how Id should be Serialized|DefaultIdHeader|
|IIdResolver|Defines how Packets should be mapped to Ids|DelegateIdResolver|
|IClient|Defines a Client|TcpClient in SimpleTCP|
|IServer|Defines a Server|TcpServer in SimpleTCP|

## Building
To Build this Project you will need to be able to run a Cake Build Script.
Additionally .Net Core 2.1 will be needed.

All Tests Target .Net Core 2.1, and all Libary parts target .Net Standard 2.0

## Contributing

See [the contributing file](CONTRIBUTING.md)!

PRs are very welcome!

## License

[MIT © Kai Jellinghaus](./LICENSE)
