import React from 'react';
import { NavLink } from 'react-router-dom';
import { LayoutDashboard, History, MapPin, Sparkles, Building2 } from 'lucide-react';

const navItems = [
  { icon: LayoutDashboard, label: 'Dashboard', path: '/' },
  { icon: History, label: 'History & Logs', path: '/history' },
  { icon: MapPin, label: 'City Lookup', path: '/city-details' },
  { icon: Sparkles, label: 'Predictor', path: '/predictor', comingSoon: true },
];

const Sidebar = () => {
  return (
    <div className="w-64 bg-slate-900 text-slate-300 flex flex-col shadow-xl z-10">
      <div className="h-16 flex items-center px-6 border-b border-slate-800 text-white font-bold text-lg gap-2">
        <Building2 className="w-6 h-6 text-indigo-500" />
        <span>RealData</span>
      </div>
      <div className="flex-1 py-6 px-4 flex flex-col gap-2">
        <p className="px-2 text-xs font-semibold text-slate-500 uppercase tracking-wider mb-2">Main Menu</p>
        {navItems.map((item) => (
          <NavLink
            key={item.label}
            to={item.comingSoon ? '#' : item.path}
            className={({ isActive }) =>
              `flex items-center gap-3 px-3 py-2.5 rounded-lg transition-all duration-200 ${
                isActive && !item.comingSoon
                  ? 'bg-indigo-600/10 text-indigo-400 font-medium'
                  : 'hover:bg-slate-800 hover:text-white'
              } ${item.comingSoon ? 'opacity-50 cursor-not-allowed hover:bg-transparent hover:text-slate-300' : ''}`
            }
            onClick={(e) => {
              if (item.comingSoon) e.preventDefault();
            }}
          >
            <item.icon className="w-5 h-5" />
            <span>{item.label}</span>
            {item.comingSoon && (
              <span className="ml-auto text-[10px] font-bold uppercase tracking-wider bg-slate-800 text-slate-400 px-2 py-0.5 rounded-full">
                Soon
              </span>
            )}
          </NavLink>
        ))}
      </div>
      <div className="p-4 border-t border-slate-800">
        <div className="bg-slate-800 rounded-xl p-4 flex flex-col items-start text-sm">
          <span className="text-white font-medium mb-1">Pro Plan</span>
          <span className="text-slate-400 text-xs mb-3">Unlock all features</span>
          <button className="w-full bg-indigo-600 hover:bg-indigo-500 text-white font-medium py-1.5 rounded-lg transition-colors text-xs">
            Upgrade
          </button>
        </div>
      </div>
    </div>
  );
};

export default Sidebar;
