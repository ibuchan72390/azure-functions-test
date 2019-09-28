# Infrastructure

To the best that I can understand, this section of the structure is entirely pointless in function design.

When working with an IoC container, the Infrastructure project contains the implementations of your interfaces and configurations objects.

Since function design is more of a flow structure, there is less of a focus on consumption of implementations and more of the simple passing of messages. With this structure, the actual implementation never needs to be known by any layer; the only connection to the consumption of another function is that model that is consumed.  This makes it so the domain-layer model is the only true interface that any function needs to know in order to interact with another function.

It should be noted that circular references are still possible in this flow structure, it's simply an infinitely passing message.