using System.ComponentModel.DataAnnotations;

namespace Devices
{
    public enum Command
    {
        /// <summary>
        /// The command creates a connection between the application and a local or remote EDIsecure. The command sets also the options for data compression and data encryption of the resulting connection. EDIsecure supports the XTEA encryption and the LZ4 compression. A local connection doesn’t need compression and encoding, for a remote connection is suggested to use both of them.
        /// A local connection can use the loopback address 127.0.0.1 or the real address of the PC.
        /// <para>Note: Using a local connection without data compression and encryption, the “connect” and “disconnect” commands are optional and the following commands doesn’t need the “connection_handle” parameter. If the application must contact different addresses (local and remote), is suggested to use always the “connect” and “disconnect” commands to have a unique code for both kinds of connection.</para>
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code><![CDATA[
        /// <command name="connect" server="127.0.0.1" application_id="123" />
        /// ]]></code></para>
        /// <list type="bullet">
        /// <item>command name - connect</item>
        /// <item>server - IP-address of the local or a Smart Device Line (or address of a remote PC running EDIsecure)</item>
        /// <item>application_id - A number to identify which application has generated this job (useful if more application are running on the same client)</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Output XML:<para>
        /// <code><![CDATA[
        /// <result accepted="true" connection_handle="104759844" />
        /// ]]></code></para>
        /// connection_handle - An integer representing this connection. This handle must be transmitted with all following commands. The handle is valid until calling the “disconnect” command.
        /// </returns>
        [Display(Name = "Connect")]
        CONNECT,

        /// <summary>
        /// This function must be called to terminate the connection between the client and the server.
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code><![CDATA[
        /// <command name="disconnect" connection_handle="104759844" />
        /// ]]></code></para>
        /// <list type="bullet">
        /// <item>command name - disconnect</item>
        /// <item>connection_handle - A valid handle for an open connection</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Output XML:<para>
        /// <code><![CDATA[
        /// <result accepted="true" />
        /// ]]></code></para>
        /// </returns>
        [Display(Name = "Disconnect")]
        DISCONNECT,

        /// <summary>
        /// The client has to send this command after transmitting the XML-job and its resources. (If a notification suspends the job, the client has to send resume_xml_job.).
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code><![CDATA[
        /// <command name="start_xml_job" job_id="2" connection_handle="104759844" />
        /// ]]></code></para>
        /// <list type="bullet">
        /// <item>command name - start_xml_job</item>
        /// <item>job_id - The ID of the job to be produced</item>
        /// <item>connection_handle - A valid handle for an open connection</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Output XML:<para>
        /// <code><![CDATA[
        /// <result accepted="true" />
        /// ]]></code></para>
        /// </returns>
        [Display(Name = "Start Job")]
        START_JOB,

        [Display(Name = "Add new Job")]
        ADD_NEW_XML_JOB,

        [Display(Name = "Read card position")]
        READ_POSITION,

        /// <summary>
        /// The client has to send this command after transmitting the XML-job and its resources. (If a notification suspends the job, the client has to send resume_xml_job.).
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code><![CDATA[
        /// <command name="get_printer_status"><printer port ="USB" port_number="1" hardware_type="XID8300DS" /></command>
        /// ]]></code></para>
        /// <list type="bullet">
        /// <item>command name - start_xml_job</item>
        /// <item>job_id - The ID of the job to be produced</item>
        /// <item>connection_handle - A valid handle for an open connection</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Output XML:<para>
        /// <code><![CDATA[
        /// <result accepted="true">
        /// 	<status value="Busy" paused="false" simulated="false">
        /// 		<sub_status value="ContactlessEncoderPosition" />
        /// 	</status>
        /// 	<hardware core="3000" type="XID8300DS" />
        /// 	<custom ribbon_type="YMCK" dual_printing_enabled="true" laminator_connected="false" firmware="V11-01B " card="Any Cards" ink="47" ink_max="50" ink_panel_set_count="1000" ink_lot_number="YD4141" film="9" film_max="10" mg_option="None" additional_sense_code="0" sense_key="0" mac_address="90-3D-68-02-25-56" door_status="Unlocked" is_print_mac_address_option_available="true" free_count="47" total_count="47" head_count="169" cleaning_count="47" error_count="102" />
        /// </result>
        /// ]]></code></para>
        /// </returns>
        [Display(Name = "Get Printer Status")]
        GET_PRINTER_STATUS,

        [Display(Name = "Position card")]
        POSITION_CARD,

        /// <summary>
        /// The client has to send this command after transmitting the XML-job and its resources. (If a notification suspends the job, the client has to send resume_xml_job.).
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code><![CDATA[
        /// <command name="get_printer_status"><printer port ="USB" port_number="1" hardware_type="XID8300DS" /></command>
        /// ]]></code></para>
        /// <list type="bullet">
        /// <item>command name - start_xml_job</item>
        /// <item>job_id - The ID of the job to be produced</item>
        /// <item>connection_handle - A valid handle for an open connection</item>
        /// </list>
        /// </remarks>
        /// <returns>
        /// Output XML:<para>
        /// <code><![CDATA[
        /// <result accepted="true" />
        /// ]]></code></para>
        /// </returns>
        [Display(Name = "Reject card")]
        POSITION_CARD_REJECTPOSITION,

        [Display(Name = "Card reposition")]
        POSITION_CARD_CONTACTLESSENCODERPOSITION,

        [Display(Name = "Wake up")]
        WAKE_UP,

        /// <summary>
        /// Creates a new Device Line Group (=EDIsecure printer) for printing with EDIsecure.
        /// </summary>
        /// <remarks>
        /// <para> Input XML:
        /// <code>
        /// <![CDATA[
        /// <command name="add_group" group="XID8300DS">
        ///     <group>
        ///         <production_line name="XID8300xx_standalone">
        ///             <devices device_id="1">
        ///                 <device hardware_type="XID8300DS"/>
        ///             </devices>
        ///         </production_line>
        ///     </group>
        /// </command>
        /// ]]>
        /// </code></para>
        /// </remarks>
        /// <returns>
        /// Output XML:
        /// <code><![CDATA[
        /// <result accepted="true" />
        /// ]]></code>
        /// </returns>
        [Display(Name = "Add group")]
        ADD_GROUP,

        /// <summary>
        /// XML output describing the result.
        /// This XML structure represents the output result with accepted="true".
        /// </summary>
        /// <remarks>
        /// Input command:
        /// <code>
        /// <![CDATA[
        /// <command name="query_groups" />
        /// ]]>
        /// </code>
        /// </remarks>
        /// <returns>
        /// <code>
        /// <![CDATA[
        /// <result accepted="true">
        ///     <groups>
        ///         <group name="XID8300DS" is_name_sealed="false" production_line="XID8300xx_standalone" />
        ///     </groups>
        /// </result>
        /// ]]>
        /// </code>
        /// </returns>
        [Display(Name = "Query groups")]
        QUERY_GROUPS,

        /// <summary>
        /// Removes a production line.
        /// Note: This is similar to deleting a printer in Windows, but this command also removes the configuration data linked with the printer.
        /// </summary>
        /// <remarks>
        /// Input command:
        /// <code>
        /// <![CDATA[
        /// <command name = "delete_group" group = "XID8300DS" />
        /// ]]>
        /// </code>
        /// </remarks>
        /// <returns>
        /// Output XML:
        /// <code><![CDATA[
        /// <result accepted="true" />
        /// ]]></code>
        /// </returns>
        [Display(Name = "Delete groups")]
        DELETE_GROUP,

        /// <summary>
        /// Sets the listed device lines to the specified group, overwriting any existing group assignments.
        /// </summary>
        /// <remarks>
        /// <para>Use this method to assign the provided device lines to a specific group.</para>
        /// <para>Input command:</para>
        /// <code><![CDATA[
        /// <command name="set_lines_to_group" group="XID8300DS" remote_name="" is_name_sealed="False">
        ///     <lines production_line_type="XID8300xx_standalone">
        ///         <line display_name="XID8300DS_Line">
        ///             <printer port="USB" port_number="1" hardware_type="XID8300DS"/>
        ///         </line>
        ///     </lines>
        /// </command>
        /// ]]></code>
        /// </remarks>
        /// <returns>
        /// <code><![CDATA[
        /// <result accepted="true">
        ///     <lines>
        ///         <line line_id="18" display_name="XID8300DS_Line" production_line_type="XID8300xx_standalone">
        ///             <printers>
        ///                 <printer device_id="0" port="USB" port_number="1" hardware_type="XID8300DS" branded_hardware_type="XID 8300 (DS)" />
        ///             </printers>
        ///         </line>
        ///     </lines>
        /// </result>
        /// ]]></code>
        /// </returns>
        [Display(Name = "Set lines")]
        SET_LINES_TO_GROUP,


        /// <summary>
        /// Retrieves all Device Lines reading the file
        /// </summary>
        /// <remarks>
        /// Input command:
        /// <code>
        /// <![CDATA[
        /// <command name="get_all_lines"/>
        /// ]]>
        /// </code>
        /// </remarks>
        /// <returns>
        /// <code>
        /// <![CDATA[
        /// <result accepted="true">
        ///     <lines>
        ///         <line line_id="9" display_name="XID 8300 (DS) Line 1" production_line_type="XID8300xx_standalone" script_template="ScriptTemplateXID.lua">
        ///             <printers>
        ///                 <printer device_id="1" port="USB" port_number="1" hardware_type="XID8300DS" branded_hardware_type="XID 8300 (DS)" />
        ///             </printers>
        ///         </line>
        ///     </lines>
        /// </result>
        /// ]]>
        /// </code>
        /// </returns>
        [Display(Name = "Get all lines")]
        GET_ALL_LINES,

        [Display(Name = "Member card")]
        MEMBER_TEMPLATE
    }

    public enum PrinterStatus
    {
        [Display(Name = "offline", Description = "Printer is offline.")]
        OFFLINE,

        [Display(Name = "busy", Description ="Busy")]
        BUSY,

        [Display(Name = "error", Description = "Printer is in an error state.")]
        ERROR,

        [Display(Name = "ready", Description ="Printer is ready")]
        READY,

        [Display(Name = "unknown", Description ="Unkown status")]
        UNKNOWN,
        
        [Display(Name = "ContactlessEncoderPosition", Description = "Card is in encoding position")]
        BUSY_CONTACTLESSENCODERPOSITION,

        [Display(Name = "Printing", Description = "Printer is printing.")]
        BUSY_PRINTING,

        [Display(Name = "ejecting", Description ="Card is being ejected")]
        BUSY_EJECTING,

        [Display(Name = "RejectPosition", Description ="Card is being rejected")]
        BUSY_REJECTPOSITION,

        [Display(Name = "busy", Description = "Printer is ready.")]
        None ,
        
        [Display(Name = "PaperProblem", Description = "Printer has a paper problem.")]
        PaperProblem ,
        
        [Display(Name = "NoToner", Description = "Printer is out of toner.")]
        NoToner,
        
        [Display(Name = "DoorOpen", Description = "Printer's door is open.")]
        DoorOpen ,
        
        [Display(Name = "busy", Description = "Printer is out of memory.")]
        OutOfMemory,
        
        [Display(Name = "busy", Description = "Printer's output bin is full.")]
        OutputBinFull,
        
        [Display(Name = "busy", Description = "Printer is out of paper.")]
        PaperOut,
        
        [Display(Name = "busy", Description = "Printer is paused.")]
        Paused,
        
        [Display(Name = "busy", Description = "Printer toner is low.")]
        TonerLow ,
        
        [Display(Name = "busy", Description = "Printer requires user intervention.")]
        UserIntervention ,
        
        [Display(Name = "busy", Description = "Printer requires manual feed.")]
        ManualFeed,

        [Display(Name = "warming", Description ="Printer is warming up")]
        WarmingUp,
    }

    public enum PrinterAttributes {
        [Display(Name = "accepted")]
        ACCEPTED,

        [Display(Name = "connection_handle")]
        CONNECTION_HANDLE,

        [Display(Name = "value")]
        VALUE,

        [Display(Name = "position")]
        POSITION,

        [Display(Name = "xml_job")]
        XML_JOB,

        [Display(Name = "job_id")]
        JOB_ID,

        [Display(Name = "error")]
        ERROR,

        [Display(Name = "status")]
        STATUS,

        [Display(Name = "translated_error")]
        TRANSLATED_ERROR
    }
}