﻿using ApplicationCommon.DTOs.Image;
using Newtonsoft.Json;

namespace ApplicationCommon.DTOs.User;

public class UserUpdateDTO
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Bio { get; set; }
    public string Location { get; set; }
    public ImageDTO? Avatar { get; set;  }
}