#pragma once

#include <string>
#include <time.h>
#include <chrono>
#include <numeric>
#include <vector>
#include <Core/CoreAll.h>
#include <Fusion/FusionAll.h>
#include <CAM/CAMAll.h>
#include <Core/UserInterface/ToolbarControls.h>
#include <Core/UserInterface/Command.h>
#include <Core/UserInterface/CommandEvent.h>
#include <Core/UserInterface/CommandEventHandler.h>
#include <Core/UserInterface/CommandEventArgs.h>
#include "AddIn/EUI.h"
#include "Data/BXDA/Mesh.h"
#include "Data/BXDJ/ConfigData.h"
#include "Data/BXDJ/RigidNode.h"
#include "Data/BXDA/BinaryWriter.h"
#include "Data/BXDJ/XmlWriter.h"

using namespace adsk::core;
using namespace adsk::fusion;
using namespace adsk::cam;

namespace Synthesis
{
	enum logLevels { info, warn, critikal };

	class Exporter
	{
	public:
		Exporter(Ptr<Application>);
		~Exporter();

		void exportExample();
		void exportExampleXml();
		void exportMeshes();

	private:
		Ptr<Application> fusionApplication;

	};
}
