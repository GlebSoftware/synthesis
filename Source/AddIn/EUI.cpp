//
//  EUI.cpp
//  FusionSynth
//
//  Created by Madvisitor on 4/16/18.
//  Copyright © 2018 Autodesk. All rights reserved.
//

#include "EUI.h"

using namespace Synthesis;

EUI::EUI()
{

}

EUI::EUI(Ptr<UserInterface> UI, Ptr<Application> APP)
{
	UI = UI;
	app = APP;
	createWorkspace();
};

EUI::~EUI()
{

}

bool EUI::createWorkspace()
{
	try
	{
		// Create workspace
		workSpace = UI->workspaces()->add(app->activeProduct()->productType(), "10001", "Synthesis Exporter", "Resources/Sample");
		workSpace->tooltip("Export robot models to the Synthesis engine");
		
		// Create toolbar
		Ptr<ToolbarPanels> toolbarPanels = workSpace->toolbarPanels();
		toolbar = workSpace->toolbarPanels()->add("SynthesisExport", "Export");
		toolbarControls = toolbar->controls();

		// Create buttons
		if (!defineExportButton())
			throw "Failed to create toolbar buttons.";

		// Add buttons to toolbar
		toolbarControls->addCommand(exportButtonCommand)->isPromoted(true);
		
		// Activate workspace
		workSpace->activate();

		return true;
	}
	catch (std::exception e)
	{
		UI->messageBox("Failed to load Synthesis Exporter add-in.");
		return false;
	}
}

bool EUI::defineExportButton()
{
	// Create button command definition
	exportButtonCommand = UI->commandDefinitions()->addButtonDefinition("CustomPanelExport", "Setup your robot for exporting to Synthesis.", "Resources/Sample");

	// Add create and click events to button
	Ptr<CommandCreatedEvent> commandCreatedEvent = exportButtonCommand->commandCreated();
	if (!commandCreatedEvent)
		return;
	
	ShowPaletteCommandCreatedHandler* commandCreatedEventHandler = new ShowPaletteCommandCreatedHandler;
	commandCreatedEventHandler->app = app;
	
	return commandCreatedEvent->add(commandCreatedEventHandler);
}
