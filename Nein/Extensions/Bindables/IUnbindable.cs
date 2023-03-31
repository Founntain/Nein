﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Nein.Extensions.Bindables;

public interface IUnbindable
{
    void UnbindEvents();

    void UnbindBindings();

    void UnbindAll();

    void UnbindFrom(IUnbindable other);
}