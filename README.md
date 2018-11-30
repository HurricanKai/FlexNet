

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

## API
To extend FlexNet, you can write one of the following, depending on your needs:

|Name|Description|Offically Provided|
|--|--|--|
|ILengthBehaviour|Defines how Servers should learn about Length|DefaultDynamicInt32LengthBehaviour, ConstantLengthBehaviour in Core|
|IDelegateBuilder|Defines how Read/Write Delegates are Build|
|IIdHeader|Defines how Id should be Read|DefaultIdHeader|

## More optional sections

## Contributing

See [the contributing file](CONTRIBUTING.md)!

PRs accepted.

Small note: If editing the Readme, please conform to the [standard-readme](https://github.com/RichardLitt/standard-readme) specification.

### Any optional sections

## License

[MIT © Richard McRichface.](../LICENSE)
<!--stackedit_data:
eyJoaXN0b3J5IjpbMTc3NjU5NjY1Miw0MTY0ODcxMDNdfQ==
-->