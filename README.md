# Teamcenter SOA Classification Patch for TC12.2

## Overview

This repository provides a critical patch for a bug in **Teamcenter SOA (Service-Oriented Architecture)**, when performing classification-related operations in **Teamcenter 12.2**. The bug caused SOA services to crash during operations involving certain classification methods, resulting in instability and preventing normal functionality.

## Problem Description

### The Issue
Teamcenter SOA processes classification data by constructing and managing internal data structures called **nodes**. Each node corresponds to a specific data type or object in the system. The original implementation reused existing nodes based solely on their **type**, without considering their **name**.

This led to a critical bug where nodes of the same type but with different names were incorrectly reused. This caused conflicts and resulted in system crashes, especially during classification operations.

### Symptoms of the Bug:
- SOA services crashed during classification-related workflows.
- Classification operations that involved multiple nodes of the same type but different names would fail.
- This made it impossible to complete classification tasks reliably, disrupting core functionalities.

### Affected Version:
- **Teamcenter 12.2**

## Why This Was Problematic

Classification is a key function in Teamcenter, essential for organizing and managing data. Without a reliable way to process classification operations, users were unable to manage critical data, leading to disruptions in business workflows.

## The Solution

### Patch Summary
The patch modifies how the system checks for node reuse. The new implementation in the `Teamcenter.Soa.Internal.Client.Parse.Node_patch.BuildChildNode` (`TcSoaClient.dll`) method now compares both the **type** and the **name** of each node before deciding whether to reuse it. This ensures that nodes with the same type but different names are correctly handled as separate entities.

#### Key Fix:
1. **Name-Based Differentiation**: In the patched method, the logic is updated to ensure that both the **type** and **LocalName** of a node are checked. This prevents the system from incorrectly reusing nodes that share the same type but differ in name.
2. **Detour Mechanism**: Instead of modifying the core Teamcenter SOA libraries directly, this patch uses a **detour utility** to replace the original `BuildChildNode` method with the patched version at runtime. This makes the solution easy to apply without altering Teamcenter’s internal code.

### Why This Fix Works:
- **Accurate Node Reuse**: By adding a name-based check, the patch ensures that nodes are reused correctly, only when they match both the **type** and the **name**.
- **Stability Restored**: The proper node reuse logic prevents crashes, allowing classification operations to be completed without errors.

## Usage

### Original Method: 
`Teamcenter.Soa.Internal.Client.Parse.Node.BuildChildNode`
[Node.cs](./Node.cs)

```csharp
protected static Node BuildChildNode(Type type, string name, FieldInfo field, Hashtable allNodes)
{
    if (allNodes.Contains((object)type))
        return (Node)allNodes[(object)type];
    
    ...
}
```

### Patched Method: 
`Teamcenter.Soa.Internal.Client.Parse.Node_patch.BuildChildNode`
[Node_patch.cs](./Node_patch.cs)

```csharp
protected static Node BuildChildNode( Type type, string name, FieldInfo field, Hashtable allNodes)
{
    if (allNodes.Contains(type))
    {
        if (((Node)allNodes[type]).LocalName == name)
        {
            return (Node)allNodes[type];
        }
    }

    ...
}  
```

### Applying the Patch Using [DetourUtility](https://github.com/V4SS3UR/DetourUtility):

```csharp
// Get MethodInfo for the source and destination methods
MethodInfo sourceMethod = typeof(Teamcenter.Soa.Internal.Client.Parse.Node).GetMethod("BuildChildNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
MethodInfo destinationMethod = typeof(Teamcenter.Soa.Internal.Client.Parse.Node_patch).GetMethod("BuildChildNode", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

// Apply the detour
DetourUtility.TryDetourFromTo(sourceMethod, destinationMethod);
```

## Acknowledgments

- **Teamcenter SOA API Documentation** for providing the necessary references.
- **Method detouring concept contributors** for enabling the seamless detour mechanism.