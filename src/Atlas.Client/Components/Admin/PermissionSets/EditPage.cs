﻿using System;
using System.Threading.Tasks;
using Atlas.Models.Admin.PermissionSets;
using Microsoft.AspNetCore.Components;

namespace Atlas.Client.Components.Admin.PermissionSets
{
    public abstract class EditPage : AdminPageBase
    {
        [Parameter] public Guid Id { get; set; }

        protected FormComponentModel Model { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            Model = await ApiService.GetFromJsonAsync<FormComponentModel>($"api/admin/permission-sets/edit/{Id}");
        }

        protected async Task UpdateAsync()
        {
            await ApiService.PostAsJsonAsync("api/admin/permission-sets/update", Model.PermissionSet);
            NavigationManager.NavigateTo("/admin/permission-sets");
        }
    }
}