AsyncLogging
============

IIS Module to log all incoming requests and responses


SQL Server config
=================
```xml
<configuration>
  <configSections>
    <sectionGroup name="applicationSettings" type="System.Configuration.ApplicationSettingsGroup, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" >
      <section name="AsyncLogging.Properties.Settings" type="System.Configuration.ClientSettingsSection, System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" requirePermission="false" />
    </sectionGroup>
  </configSections>
  <connectionStrings>
    <add name="ConnName" connectionString="..." providerName="System.Data.SqlClient" />
  </connectionStrings>
  <applicationSettings>
    <AsyncLogging.Properties.Settings>
      <setting name="LoggerType" serializeAs="String">
        <value>SqlServer</value>
      </setting>
      <setting name="ConnectionName" serializeAs="String">
        <value>ConnName</value>
      </setting>
      <setting name="Enabled" serializeAs="String">
        <value>false</value>
      </setting>
      <setting name="StatusCodes" serializeAs="String">
        <value>200|500|600</value>
      </setting>
      <setting name="SqlInsertStatement" serializeAs="String">
        <value>
          <!--INSERT [ServerRequestLogs]...-->
        </value>
      </setting>
      <setting name="ContentTypes" serializeAs="String">
        <value>json|xml|html|text</value>
      </setting>
    </AsyncLogging.Properties.Settings>
  </applicationSettings>
</configuration>
```
