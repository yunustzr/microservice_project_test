﻿namespace AuthenticationApi.Domain.Models.ENTITY
{
    public class SystemSetting
    {
        public Guid Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

}
