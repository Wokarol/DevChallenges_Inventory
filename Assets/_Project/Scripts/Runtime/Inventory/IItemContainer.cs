﻿using System;
using System.Collections.Generic;

public interface IItemContainer
{
    ItemStack this[int index] { get; set; }

    int SlotCount { get; }

    event Action InventoryUpdated;
}