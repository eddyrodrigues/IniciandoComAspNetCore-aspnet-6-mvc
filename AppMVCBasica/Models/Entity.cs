﻿using System;

namespace AppMVCBasica.Models;

public abstract class Entity
{
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    public Guid Id { get; set; }
}